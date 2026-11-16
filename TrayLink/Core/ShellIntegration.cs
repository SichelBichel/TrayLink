using Microsoft.Win32;

namespace TrayLink.Core
{
    /// <summary>
    /// "Add to TrayLink" in the Explorer context menu for files and folders.
    /// Registered per user under HKCU\Software\Classes, so no admin rights are needed.
    /// The menu entry calls TrayLink.exe --add "%1" (no separate helper exe, no console window).
    /// </summary>
    internal static class ShellIntegration
    {
        private const string VerbName = "AddToTrayLink";

        private static readonly string[] ShellRoots =
        {
            @"Software\Classes\*\shell\",
            @"Software\Classes\Directory\shell\"
        };

        private static string CommandLine => $"\"{AppPaths.ExePath}\" --add \"%1\"";

        public static bool IsRegistered
        {
            get
            {
                try
                {
                    foreach (var root in ShellRoots)
                    {
                        using var cmd = Registry.CurrentUser.OpenSubKey(root + VerbName + @"\command");
                        if (cmd?.GetValue("") is not string value ||
                            !value.Equals(CommandLine, StringComparison.OrdinalIgnoreCase))
                            return false;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void Register()
        {
            foreach (var root in ShellRoots)
            {
                using var key = Registry.CurrentUser.CreateSubKey(root + VerbName);
                key.SetValue("", "Add to TrayLink");
                key.SetValue("Icon", $"\"{AppPaths.ExePath}\",0");

                using var command = key.CreateSubKey("command");
                command.SetValue("", CommandLine);
            }
        }

        public static void Unregister()
        {
            foreach (var root in ShellRoots)
                Registry.CurrentUser.DeleteSubKeyTree(root + VerbName, throwOnMissingSubKey: false);
        }

        /// <summary>
        /// If the registered exe no longer exists (old helper exe, moved install) point the entry
        /// to this exe. A still valid registration of another install is left alone.
        /// </summary>
        public static void RepairIfStale()
        {
            try
            {
                using var cmd = Registry.CurrentUser.OpenSubKey(ShellRoots[0] + VerbName + @"\command");
                if (cmd?.GetValue("") is string value &&
                    !value.Equals(CommandLine, StringComparison.OrdinalIgnoreCase) &&
                    !File.Exists(ExtractExe(value)))
                    Register();
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>Returns the exe of a command line like "C:\x\a.exe" --add "%1".</summary>
        internal static string ExtractExe(string commandLine)
        {
            var s = commandLine.Trim();
            if (s.StartsWith('"'))
            {
                int end = s.IndexOf('"', 1);
                return end > 1 ? s[1..end] : s.Trim('"');
            }
            int space = s.IndexOf(' ');
            return space > 0 ? s[..space] : s;
        }
    }
}
