using Microsoft.Win32;

namespace TrayLink.Core
{
    /// <summary>Start with Windows via HKCU\...\Run (per user, no admin rights).</summary>
    internal static class Autostart
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "TrayLink";

        private static string Command => $"\"{AppPaths.ExePath}\"";

        public static bool IsEnabled
        {
            get
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(RunKey);
                    return key?.GetValue(ValueName) is string;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void SetEnabled(bool enabled)
        {
            using var key = Registry.CurrentUser.CreateSubKey(RunKey);
            if (enabled)
                key.SetValue(ValueName, Command);
            else
                key.DeleteValue(ValueName, throwOnMissingValue: false);
        }

        public static void RepairIfStale()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKey);
                if (key?.GetValue(ValueName) is string value &&
                    !value.Equals(Command, StringComparison.OrdinalIgnoreCase) &&
                    !File.Exists(ShellIntegration.ExtractExe(value)))
                    SetEnabled(true); // registered exe is gone (moved install), point to this one
            }
            catch
            {
                // ignore
            }
        }
    }
}
