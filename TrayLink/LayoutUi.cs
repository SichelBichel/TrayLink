using TrayLink.Core;
using TrayLink.Core.Layouts;

namespace TrayLink
{
    /// <summary>Save / restore actions shared by the tray menu, the popup and the layout manager.</summary>
    internal static class LayoutUi
    {
        public static bool Busy { get; private set; }

        /// <summary>A layout was saved, renamed or deleted (tray menu and manager refresh).</summary>
        public static event Action? LayoutsChanged;

        public static void NotifyChanged() => LayoutsChanged?.Invoke();

        /// <summary>Asks for a name and saves the open windows. Null when cancelled or nothing to save.</summary>
        public static async Task<DesktopLayout?> SaveCurrentAsync(IWin32Window? owner)
        {
            var name = NamePromptForm.Ask(owner, "Save desktop layout", "Name of the layout:", SuggestName());
            if (name == null)
                return null;

            var old = LayoutStore.Load(name);
            if (old != null &&
                MessageBox.Show(owner, $"Replace the layout '{name}' with the windows that are open now?\n\n" +
                    "Use, Start, Delay and own \"Start with\" values of known windows are kept.",
                    "Save desktop layout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return null;

            return await CaptureAndSaveAsync(name, old, owner);
        }

        /// <summary>Takes the open windows again for an existing layout.</summary>
        public static Task<DesktopLayout?> UpdateAsync(DesktopLayout old, IWin32Window? owner) =>
            CaptureAndSaveAsync(old.Name, old, owner);

        private static async Task<DesktopLayout?> CaptureAndSaveAsync(string name, DesktopLayout? old, IWin32Window? owner)
        {
            if (Busy)
                return null;
            Busy = true;
            try
            {
                var layout = await StaRunner.Run(() => WindowCapture.Capture(name));
                if (layout.Windows.Count == 0)
                {
                    MessageBox.Show(owner, "No windows found to save.", "Save desktop layout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }

                if (old != null)
                    CarryOver(old, layout);

                LayoutStore.Save(layout);
                NotifyChanged();

                int startable = layout.Windows.Count(w => w.CanStart);
                Program.ShowInfo("Desktop layout saved",
                    $"'{name}': {layout.Windows.Count} window(s) on {layout.Monitors.Count} monitor(s), " +
                    $"{startable} can be started, the others are only placed.");
                return layout;
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Could not save the layout:\n" + ex.Message, "Save desktop layout",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                Busy = false;
            }
        }

        /// <summary>Keeps what the user set for a window that is in the old and in the new snapshot.</summary>
        public static void CarryOver(DesktopLayout old, DesktopLayout fresh)
        {
            var unused = old.Windows.ToList();
            foreach (var w in fresh.Windows)
            {
                var match =
                    unused.FirstOrDefault(o => SameExe(o, w) && o.Launch == LaunchKind.Explorer && w.Launch == LaunchKind.Explorer &&
                                               string.Equals(o.StartWith, w.StartWith, StringComparison.OrdinalIgnoreCase)) ??
                    unused.FirstOrDefault(o => SameExe(o, w) && o.Title == w.Title) ??
                    unused.FirstOrDefault(o => SameExe(o, w) && o.Launch == w.Launch);
                if (match == null)
                    continue;

                unused.Remove(match);
                w.Include = match.Include;
                w.DelaySeconds = match.DelaySeconds;
                if (w.CanStart)
                    w.Start = match.Start;

                // A project path or URL typed by hand (DAW, browser) survives a new snapshot.
                if (w.Launch == LaunchKind.Exe && match.Launch == LaunchKind.Exe &&
                    string.IsNullOrWhiteSpace(w.StartWith) && !string.IsNullOrWhiteSpace(match.StartWith))
                    w.StartWith = match.StartWith;
            }
        }

        private static bool SameExe(LayoutWindow a, LayoutWindow b) =>
            string.Equals(a.ExePath, b.ExePath, StringComparison.OrdinalIgnoreCase) ||
            (a.Launch == LaunchKind.Squirrel && b.Launch == LaunchKind.Squirrel &&
             string.Equals(a.StartWith, b.StartWith, StringComparison.OrdinalIgnoreCase));

        public static async Task RestoreAsync(DesktopLayout layout, bool atLogin)
        {
            if (Busy)
                return;
            Busy = true;
            try
            {
                Program.ShowInfo("Restoring desktop layout", $"'{layout.Name}': starting and placing {layout.Windows.Count(w => w.Include)} window(s) ...");
                var report = await LayoutRestorer.RestoreAsync(layout, atLogin);
                Program.ShowInfo($"Layout '{layout.Name}' restored", report.Summary,
                    report.NotFound.Count + report.Failed.Count > 0 ? ToolTipIcon.Warning : ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                Program.ShowInfo("Restore failed", ex.Message, ToolTipIcon.Error);
            }
            finally
            {
                Busy = false;
            }
        }

        private static string SuggestName()
        {
            if (!LayoutStore.Exists("Default"))
                return "Default";
            for (int i = 2; ; i++)
                if (!LayoutStore.Exists("Layout " + i))
                    return "Layout " + i;
        }

        // ------------------------------------------------------------------
        // Restore when Windows starts
        // ------------------------------------------------------------------

        /// <summary>
        /// Called once at the start of TrayLink. Restores the chosen layout once per Windows sign in:
        /// only when TrayLink started shortly after the shell (autostart) and not yet for this shell session.
        /// </summary>
        public static void ScheduleAutoRestore()
        {
            var name = AppSettings.Current.AutoRestoreLayout;
            if (string.IsNullOrWhiteSpace(name))
                return;

            var shellStart = ShellStartTimeUtc();
            if (shellStart == null)
                return;

            var sessionKey = shellStart.Value.ToString("O");
            if (AppSettings.Current.LastAutoRestoreSession == sessionKey)
                return; // already done for this sign in (TrayLink restarted, update)

            if (DateTime.UtcNow - shellStart.Value > TimeSpan.FromMinutes(10))
                return; // TrayLink started by hand later: do not rearrange the desktop unasked

            AppSettings.Current.LastAutoRestoreSession = sessionKey;
            AppSettings.Save();

            var timer = new System.Windows.Forms.Timer { Interval = Math.Clamp(AppSettings.Current.AutoRestoreDelaySeconds, 1, 600) * 1000 };
            timer.Tick += async (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                var layout = LayoutStore.Load(name);
                if (layout != null)
                    await RestoreAsync(layout, atLogin: true);
            };
            timer.Start();
        }

        /// <summary>Start of the Explorer shell of this session = the sign in (survives Fast Startup, unlike the uptime).</summary>
        private static DateTime? ShellStartTimeUtc()
        {
            try
            {
                var shell = ShellNative.GetShellWindow();
                if (shell == IntPtr.Zero)
                    return null;
                Native.GetWindowThreadProcessId(shell, out uint pid);
                using var p = System.Diagnostics.Process.GetProcessById((int)pid);
                var t = p.StartTime.ToUniversalTime();
                return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, DateTimeKind.Utc);
            }
            catch
            {
                return null;
            }
        }

        private static class ShellNative
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr GetShellWindow();
        }
    }
}
