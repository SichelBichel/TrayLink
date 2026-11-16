#nullable enable
using TrayLink.Updater;
using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TrayLink.Updater
{
    public sealed class UpdateClient
    {
        private readonly HttpClient _http;
        private readonly Uri _baseUri;

        public UpdateClient(HttpClient http, Uri baseUri)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            if (!_baseUri.IsAbsoluteUri) throw new ArgumentException("baseUri must be absolute", nameof(baseUri));
        }

        public sealed record ClientOptions(
            bool StrictKeyStatusCheck = true,
            bool RequireSignature = true,
            TimeSpan KeyCacheTtl = default,
            int DownloadBufferSize = 1024 * 128
        );

        public sealed record Manifest(
            string ProductId,
            string Channel,
            string Version,
            string ArtifactUrl,
            string Sha256,
            long SizeBytes,
            string PublishedAt,
            string Notes,
            string? Kid,
            string? Sig
        );

        public sealed record KeyInfo(
            string Kid,
            string PubB64,
            bool IsActive,
            bool IsRevoked,
            string CreatedAt,
            string RotatedAt
        );

        public sealed record UpdateCheckResult(
            bool UpdateAvailable,
            string InstalledVersion,
            Manifest RemoteManifest
        );

        public sealed record DownloadResult(
            Manifest Manifest,
            string FilePath
        );

        private sealed record CachedKey(byte[] Pub32, bool? IsActive, bool? IsRevoked, DateTimeOffset CachedAt);

        private readonly Dictionary<string, CachedKey> _keyCache = new(StringComparer.Ordinal);
        private readonly object _cacheLock = new();

        public async Task<UpdateCheckResult> CheckForUpdateAsync(
            string productId,
            string channel,
            string installedVersion,
            ClientOptions? options = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(productId)) throw new ArgumentException("productId required", nameof(productId));
            if (string.IsNullOrWhiteSpace(channel)) throw new ArgumentException("channel required", nameof(channel));
            if (string.IsNullOrWhiteSpace(installedVersion)) installedVersion = "0.0.0";

            options ??= new ClientOptions(KeyCacheTtl: TimeSpan.FromMinutes(15));

            var manifest = await GetManifestAsync(productId, channel, ct).ConfigureAwait(false);

            if (options.RequireSignature)
            {
                if (string.IsNullOrWhiteSpace(manifest.Kid) || string.IsNullOrWhiteSpace(manifest.Sig))
                    throw new UpdaterProtocolException("Manifest is missing kid/sig but signature is required.");
            }

            if (!string.IsNullOrWhiteSpace(manifest.Kid) && !string.IsNullOrWhiteSpace(manifest.Sig))
            {
                await VerifyManifestSignatureAsync(manifest, options, ct).ConfigureAwait(false);
            }

            var updateAvailable = CompareVersions(installedVersion, manifest.Version) < 0;
            return new UpdateCheckResult(updateAvailable, installedVersion, manifest);
        }

        public async Task<DownloadResult> DownloadUpdateAsync(
            Manifest manifest,
            string stagingDirectory,
            ClientOptions? options = null,
            CancellationToken ct = default)
        {
            options ??= new ClientOptions(KeyCacheTtl: TimeSpan.FromMinutes(15));

            Directory.CreateDirectory(stagingDirectory);

            var artifactUri = new Uri(manifest.ArtifactUrl, UriKind.Absolute);

            var fileName = Path.GetFileName(artifactUri.LocalPath);
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"{manifest.ProductId}-{manifest.Version}.bin";

            var finalPath = Path.Combine(stagingDirectory, fileName);
            var partialPath = finalPath + ".partial";

            if (File.Exists(partialPath)) File.Delete(partialPath);

            using var req = new HttpRequestMessage(HttpMethod.Get, artifactUri);
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            if (resp.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException($"Artifact download failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");

            if (resp.Content.Headers.ContentLength is long cl && manifest.SizeBytes > 0 && cl != manifest.SizeBytes)
            {
            }

            await using (var net = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false))
            await using (var fs = new FileStream(
                partialPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                options.DownloadBufferSize,
                FileOptions.SequentialScan))
            {
                await net.CopyToAsync(fs, options.DownloadBufferSize, ct).ConfigureAwait(false);
            }

            var fi = new FileInfo(partialPath);
            if (manifest.SizeBytes > 0 && fi.Length != manifest.SizeBytes)
                throw new IntegrityException($"Size mismatch. Expected {manifest.SizeBytes} bytes, got {fi.Length} bytes.");

            var actualSha = await ComputeSha256HexAsync(partialPath, ct).ConfigureAwait(false);
            if (!StringEqualsHex(manifest.Sha256, actualSha))
                throw new IntegrityException($"SHA256 mismatch. Expected {manifest.Sha256}, got {actualSha}.");

            if (File.Exists(finalPath)) File.Delete(finalPath);
            File.Move(partialPath, finalPath);

            return new DownloadResult(manifest, finalPath);
        }

        public async Task<Manifest> GetManifestAsync(string productId, string channel, CancellationToken ct = default)
        {
            var uri = new Uri(_baseUri, $"/api/v1/manifest?product={Uri.EscapeDataString(productId)}&channel={Uri.EscapeDataString(channel)}");

            using var req = new HttpRequestMessage(HttpMethod.Get, uri);
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            if (resp.StatusCode == HttpStatusCode.NotFound)
                throw new UpdaterProtocolException($"No published release for product='{productId}', channel='{channel}'.");

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"Manifest request failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");

            var json = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            return ParseManifest(json);
        }

        private static Manifest ParseManifest(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string GetStr(string name, bool required = true)
            {
                if (!root.TryGetProperty(name, out var p))
                {
                    if (required) throw new UpdaterProtocolException($"Manifest missing '{name}'.");
                    return "";
                }
                if (p.ValueKind == JsonValueKind.Null)
                {
                    if (required) throw new UpdaterProtocolException($"Manifest '{name}' is null.");
                    return "";
                }
                return p.GetString() ?? "";
            }

            long GetI64(string name, bool required = true)
            {
                if (!root.TryGetProperty(name, out var p))
                {
                    if (required) throw new UpdaterProtocolException($"Manifest missing '{name}'.");
                    return 0;
                }
                if (p.ValueKind != JsonValueKind.Number) throw new UpdaterProtocolException($"Manifest '{name}' not a number.");
                return p.GetInt64();
            }

            string? GetStrOpt(string name)
            {
                if (!root.TryGetProperty(name, out var p)) return null;
                if (p.ValueKind == JsonValueKind.Null) return null;
                var s = p.GetString();
                return string.IsNullOrWhiteSpace(s) ? null : s;
            }

            return new Manifest(
                ProductId: GetStr("product_id"),
                Channel: GetStr("channel"),
                Version: GetStr("version"),
                ArtifactUrl: GetStr("artifact_url"),
                Sha256: GetStr("sha256"),
                SizeBytes: GetI64("size_bytes"),
                PublishedAt: GetStr("published_at"),
                Notes: GetStr("notes", required: false),
                Kid: GetStrOpt("kid"),
                Sig: GetStrOpt("sig")
            );
        }

        private async Task VerifyManifestSignatureAsync(Manifest manifest, ClientOptions options, CancellationToken ct)
        {
            var kid = manifest.Kid!;
            var sigB64 = manifest.Sig!;

            bool? isActive = null, isRevoked = null;

            if (options.StrictKeyStatusCheck)
            {
                var infos = await GetKeysListAsync(ct).ConfigureAwait(false);
                var match = infos.Find(k => string.Equals(k.Kid, kid, StringComparison.Ordinal));
                if (match is null)
                    throw new UpdaterProtocolException($"kid '{kid}' not found in /api/v1/keys.");

                isActive = match.IsActive;
                isRevoked = match.IsRevoked;

                if (match.IsRevoked || !match.IsActive)
                    throw new UpdaterProtocolException($"kid '{kid}' is not active (revoked={match.IsRevoked}, active={match.IsActive}).");
            }

            var pub = await GetPublicKey32Async(kid, isActive, isRevoked, options, ct).ConfigureAwait(false);

            var unsignedBytes = BuildUnsignedManifestJsonBytes(manifest);

            byte[] sig;
            try { sig = Convert.FromBase64String(sigB64); }
            catch (FormatException fe) { throw new UpdaterProtocolException("Manifest sig is not valid base64.", fe); }

            if (sig.Length != 64)
                throw new UpdaterProtocolException($"Invalid Ed25519 signature length: {sig.Length} (expected 64).");

            if (pub.Length != 32)
                throw new UpdaterProtocolException($"Invalid Ed25519 public key length: {pub.Length} (expected 32).");

            var algo = SignatureAlgorithm.Ed25519;

            NSec.Cryptography.PublicKey pubKey;
            try
            {
                pubKey = NSec.Cryptography.PublicKey.Import(algo, pub, KeyBlobFormat.RawPublicKey);
            }
            catch (Exception ex)
            {
                throw new UpdaterProtocolException("Failed to import Ed25519 public key.", ex);
            }

            if (!algo.Verify(pubKey, unsignedBytes, sig))
                throw new SignatureException("Manifest signature verification failed.");
        }

        private async Task<List<KeyInfo>> GetKeysListAsync(CancellationToken ct)
        {
            var uri = new Uri(_baseUri, "/api/v1/keys");

            using var req = new HttpRequestMessage(HttpMethod.Get, uri);
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"Keys list request failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");

            var json = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            using var doc = JsonDocument.Parse(json);

            // Server returns: { "keys": [ ... ] }
            if (!doc.RootElement.TryGetProperty("keys", out var keysEl) || keysEl.ValueKind != JsonValueKind.Array)
                throw new UpdaterProtocolException("Keys list response missing 'keys' array.");

            static string S(JsonElement el, string n)
            {
                if (!el.TryGetProperty(n, out var p)) return "";
                if (p.ValueKind == JsonValueKind.Null) return "";
                return p.GetString() ?? "";
            }

            static bool GetBool(JsonElement el, string n)
            {
                if (!el.TryGetProperty(n, out var p))
                    throw new UpdaterProtocolException($"Keys list missing '{n}'.");

                return p.ValueKind switch
                {
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => throw new UpdaterProtocolException($"Keys list field '{n}' not bool.")
                };
            }

            var result = new List<KeyInfo>();
            foreach (var el in keysEl.EnumerateArray())
            {
                result.Add(new KeyInfo(
                    Kid: S(el, "kid"),
                    PubB64: S(el, "pub_b64"),
                    IsActive: GetBool(el, "is_active"),
                    IsRevoked: GetBool(el, "is_revoked"),
                    CreatedAt: S(el, "created_at"),
                    RotatedAt: S(el, "rotated_at")
                ));
            }

            return result;
        }


        private async Task<byte[]> GetPublicKey32Async(
            string kid,
            bool? isActive,
            bool? isRevoked,
            ClientOptions options,
            CancellationToken ct)
        {
            var ttl = options.KeyCacheTtl == default ? TimeSpan.FromMinutes(15) : options.KeyCacheTtl;

            lock (_cacheLock)
            {
                if (_keyCache.TryGetValue(kid, out var cached))
                {
                    if (DateTimeOffset.UtcNow - cached.CachedAt <= ttl)
                        return (byte[])cached.Pub32.Clone();
                }
            }

            var uri = new Uri(_baseUri, $"/api/v1/keys/{Uri.EscapeDataString(kid)}");
            using var req = new HttpRequestMessage(HttpMethod.Get, uri);
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"Key fetch failed for kid '{kid}': {(int)resp.StatusCode} {resp.ReasonPhrase}");

            var pub = await resp.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
            if (pub.Length != 32)
                throw new UpdaterProtocolException($"Key endpoint returned {pub.Length} bytes for kid '{kid}' (expected 32).");

            lock (_cacheLock)
            {
                _keyCache[kid] = new CachedKey(pub, isActive, isRevoked, DateTimeOffset.UtcNow);
            }

            return pub;
        }

        private static byte[] BuildUnsignedManifestJsonBytes(Manifest m)
        {
            using var ms = new MemoryStream();
            using (var w = new Utf8JsonWriter(ms, new JsonWriterOptions
            {
                Indented = false,
                SkipValidation = false
            }))
            {
                w.WriteStartObject();

                w.WriteString("product_id", m.ProductId);
                w.WriteString("channel", m.Channel);
                w.WriteString("version", m.Version);
                w.WriteString("artifact_url", m.ArtifactUrl);
                w.WriteString("sha256", m.Sha256);
                w.WriteNumber("size_bytes", m.SizeBytes);
                w.WriteString("published_at", m.PublishedAt);
                w.WriteString("notes", m.Notes ?? "");

                w.WriteEndObject();
            }
            return ms.ToArray();
        }

        public static int CompareVersions(string a, string b)
        {
            var va = ParseSemVer(a);
            var vb = ParseSemVer(b);

            int c = va.Major.CompareTo(vb.Major); if (c != 0) return c;
            c = va.Minor.CompareTo(vb.Minor); if (c != 0) return c;
            c = va.Patch.CompareTo(vb.Patch); if (c != 0) return c;

            bool aPre = !string.IsNullOrEmpty(va.PreRelease);
            bool bPre = !string.IsNullOrEmpty(vb.PreRelease);
            if (aPre && !bPre) return -1;
            if (!aPre && bPre) return 1;
            return string.CompareOrdinal(va.PreRelease ?? "", vb.PreRelease ?? "");
        }

        private sealed record SemVer(int Major, int Minor, int Patch, string? PreRelease);

        private static SemVer ParseSemVer(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return new SemVer(0, 0, 0, null);
            s = s.Trim();
            if (s.StartsWith("v", StringComparison.OrdinalIgnoreCase)) s = s[1..];

            var plus = s.IndexOf('+');
            if (plus >= 0) s = s.Substring(0, plus);

            string? pre = null;
            var dash = s.IndexOf('-');
            if (dash >= 0)
            {
                pre = s[(dash + 1)..];
                s = s.Substring(0, dash);
            }

            var parts = s.Split('.', StringSplitOptions.RemoveEmptyEntries);
            int major = parts.Length > 0 ? SafeInt(parts[0]) : 0;
            int minor = parts.Length > 1 ? SafeInt(parts[1]) : 0;
            int patch = parts.Length > 2 ? SafeInt(parts[2]) : 0;
            return new SemVer(major, minor, patch, pre);
        }

        private static int SafeInt(string s)
            => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : 0;

        private static async Task<string> ComputeSha256HexAsync(string filePath, CancellationToken ct)
        {
            await using var fs = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                1024 * 128,
                FileOptions.SequentialScan);

            using var sha = SHA256.Create();

#if NET7_0_OR_GREATER
            var hash = await sha.ComputeHashAsync(fs, ct).ConfigureAwait(false);
#else
            var hash = sha.ComputeHash(fs);
#endif
            return ConvertToLowerHex(hash);
        }

        private static string ConvertToLowerHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        private static bool StringEqualsHex(string expected, string actual)
        {
            expected = (expected ?? "").Trim();
            actual = (actual ?? "").Trim();
            return string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class UpdaterProtocolException : Exception
    {
        public UpdaterProtocolException(string msg) : base(msg) { }
        public UpdaterProtocolException(string msg, Exception inner) : base(msg, inner) { }
    }

    public class SignatureException : Exception
    {
        public SignatureException(string msg) : base(msg) { }
    }

    public class IntegrityException : Exception
    {
        public IntegrityException(string msg) : base(msg) { }
    }
}