namespace TrayLink.Core
{
    /// <summary>
    /// Central place for all file locations. Config lives in %APPDATA%\TrayLink so it
    /// survives MSI updates and does not need admin rights (Program Files is read only).
    /// </summary>
    internal static class AppPaths
    {
        public static string DataDir { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrayLink");

        public static string ActionsFile => Path.Combine(DataDir, "actions.ini");
        public static string SettingsFile => Path.Combine(DataDir, "settings.json");

        public static string ExePath =>
            Environment.ProcessPath ?? Path.Combine(AppContext.BaseDirectory, "TrayLink.exe");

        public static void EnsureDataDir() => Directory.CreateDirectory(DataDir);

        /// <summary>
        /// Older versions stored actions.ini next to the exe or in the working directory.
        /// Copy it once into the new location so no entries get lost.
        /// </summary>
        public static void MigrateLegacyConfig(string startupDirectory)
        {
            try
            {
                EnsureDataDir();
                if (File.Exists(ActionsFile))
                    return;

                var candidates = new[]
                {
                    Path.Combine(AppContext.BaseDirectory, "actions.ini"),
                    Path.Combine(startupDirectory, "actions.ini")
                };

                foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (File.Exists(candidate))
                    {
                        File.Copy(candidate, ActionsFile, overwrite: false);
                        return;
                    }
                }
            }
            catch
            {
                // Migration is best effort. A fresh default config is created otherwise.
            }
        }
    }
}
