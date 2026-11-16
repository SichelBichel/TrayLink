#nullable enable
using TrayLink.Updater;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayLink.Updater;

internal static class Update
{
    private static readonly Uri UpdateBaseUri = new("https://updates.rehoga.ch");

    private const string ProductId = "traylink";
    private const string Channel = "stable";

    public static async Task CheckAndMaybeUpdateAsync(
        IWin32Window owner,
        CancellationToken ct,
        bool showNoUpdateMessage = false)
    {
        var installed = VersionUtil.StripBuildMeta(VersionUtil.GetInstalledVersion());

        using var http = CreateHttp();
        var client = new UpdateClient(http, UpdateBaseUri);

        UpdateClient.UpdateCheckResult check;
        try
        {
            // Zeigt das Ladefenster beim Überprüfen der Version an
            check = await BusyUi.RunAsync(owner, "Checking version...", async () =>
            {
                return await client.CheckForUpdateAsync(
                    productId: ProductId,
                    channel: Channel,
                    installedVersion: installed,
                    options: new UpdateClient.ClientOptions(
                        StrictKeyStatusCheck: true,
                        RequireSignature: true,
                        KeyCacheTtl: TimeSpan.FromMinutes(15)),
                    ct: ct);
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Update check failed: " + ex);

            if (showNoUpdateMessage)
            {
                MessageBox.Show(
                    owner,
                    ex.Message,
                    "Update check failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            return;
        }

        if (!check.UpdateAvailable)
        {
            if (showNoUpdateMessage)
            {
                MessageBox.Show(
                    owner,
                    $"You are already up to date.\n\nInstalled version: {installed}",
                    "No updates found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            return;
        }

        var msg =
            $"A new version is available:\n\n" +
            $"Installed: {installed}\n" +
            $"Available: {check.RemoteManifest.Version}\n\n" +
            $"Do you want to download and install the update now?";

        if (MessageBox.Show(
                owner,
                msg,
                "Update available",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) != DialogResult.Yes)
        {
            return;
        }

        var stagingDir = GetStagingDir(ProductId, Channel, check.RemoteManifest.Version);

        UpdateClient.DownloadResult dl;
        try
        {
            // Zeigt das Ladefenster beim Herunterladen der neuen Version an
            dl = await BusyUi.RunAsync(owner, "Downloading...", async () =>
            {
                return await client.DownloadUpdateAsync(
                    check.RemoteManifest,
                    stagingDir,
                    ct: ct);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                owner,
                ex.Message,
                "Update download failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        try
        {
            StartUpdaterAndExit(dl.FilePath, silent: false);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                owner,
                ex.Message,
                "Updater launch failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static HttpClient CreateHttp()
    {
        var handler = new SocketsHttpHandler
        {
            AutomaticDecompression =
                System.Net.DecompressionMethods.GZip |
                System.Net.DecompressionMethods.Deflate,
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer = 10
        };

        var http = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        var version = VersionUtil.StripBuildMeta(VersionUtil.GetInstalledVersion());
        http.DefaultRequestHeaders.UserAgent.ParseAdd($"TrayLink/{version}");
        http.DefaultRequestHeaders.Accept.ParseAdd("application/json");

        return http;
    }

    private static string GetStagingDir(string productId, string channel, string version)
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TrayLink",
            "Updates",
            productId,
            channel,
            version);

        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void StartUpdaterAndExit(string msiPath, bool silent)
    {
        var exeDir = AppContext.BaseDirectory;

        var appExe = Environment.ProcessPath
                    ?? throw new InvalidOperationException("Cannot determine current process path.");

        var updaterExe = Path.Combine(exeDir, "Updater.exe");
        if (!File.Exists(updaterExe))
            throw new FileNotFoundException(
                "Updater.exe not found next to TrayLink.exe",
                updaterExe);

        var pid = Process.GetCurrentProcess().Id;

        var args =
            $"--msi \"{msiPath}\" " +
            $"--wait-pid {pid} " +
            $"--kill " +
            $"--restart " +
            $"--app \"{appExe}\" " +
            (silent ? "--silent" : "");

        var psi = new ProcessStartInfo
        {
            FileName = updaterExe,
            Arguments = args,
            UseShellExecute = true,
            Verb = "runas",
            WorkingDirectory = exeDir
        };

        Process.Start(psi);
        Application.Exit();
    }
}