using System.Diagnostics;

namespace TrayLink.Core
{
    internal static class ActionRunner
    {
        public static void Execute(ActionConfig config, IWin32Window? owner)
        {
            try
            {
                var target = Environment.ExpandEnvironmentVariables(config.PathOrUrl.Trim().Trim('"'));
                if (string.IsNullOrWhiteSpace(target))
                    throw new InvalidOperationException("Path/URL is empty.");

                switch (config.ActionType.ToUpperInvariant())
                {
                    case ActionConfig.TypeUrl:
                        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
                        break;

                    case ActionConfig.TypeFile:
                        var psi = new ProcessStartInfo(target) { UseShellExecute = true };
                        var dir = SafeDirectoryName(target);
                        if (dir != null && Directory.Exists(dir))
                            psi.WorkingDirectory = dir; // tools often expect to start in their own folder
                        Process.Start(psi);
                        break;

                    case ActionConfig.TypePath:
                        if (!Directory.Exists(target) && !File.Exists(target))
                            throw new DirectoryNotFoundException($"Path not found:\n{target}");
                        // Quote the path, otherwise folders with commas or spaces open the wrong location.
                        Process.Start(new ProcessStartInfo("explorer.exe", $"\"{target}\"") { UseShellExecute = true });
                        break;

                    default:
                        MessageBox.Show(owner, $"Unknown action type: {config.ActionType}", "Action Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, $"Error executing '{config.ActionName}':\n{ex.Message}", "Execution Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string? SafeDirectoryName(string path)
        {
            try
            {
                return Path.IsPathRooted(path) ? Path.GetDirectoryName(path) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
