using System.Text.Json;

namespace TrayLink.Core
{
    /// <summary>
    /// Small JSON settings file in %APPDATA%\TrayLink. Replaces Properties.Settings,
    /// whose user.config got reset on every new version (and therefore on every update).
    /// </summary>
    internal sealed class AppSettings
    {
        public bool AutoHide { get; set; } = true;

        /// <summary>Tab of the popup that was open last ("Links" or "Desktops").</summary>
        public string PopupTab { get; set; } = "Links";

        /// <summary>Desktop layout restored once per Windows sign in (null = off).</summary>
        public string? AutoRestoreLayout { get; set; }

        /// <summary>Seconds after the start of TrayLink before the layout is restored at sign in.</summary>
        public int AutoRestoreDelaySeconds { get; set; } = 10;

        /// <summary>Start time of the shell session that was restored last (once per sign in).</summary>
        public string? LastAutoRestoreSession { get; set; }

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public static AppSettings Current { get; private set; } = new();

        public static void Load()
        {
            try
            {
                if (File.Exists(AppPaths.SettingsFile))
                {
                    var json = File.ReadAllText(AppPaths.SettingsFile);
                    Current = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
                }
            }
            catch
            {
                Current = new AppSettings();
            }
        }

        public static void Save()
        {
            try
            {
                AppPaths.EnsureDataDir();
                File.WriteAllText(AppPaths.SettingsFile, JsonSerializer.Serialize(Current, JsonOptions));
            }
            catch
            {
                // Settings are not critical, never crash because of them.
            }
        }
    }
}
