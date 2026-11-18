using System.Diagnostics;

namespace TrayLink.Core.Layouts
{
    /// <summary>
    /// Brings a saved layout back:
    ///   1. windows that are open already are matched and placed (no duplicates),
    ///   2. missing apps are started (Exe with arguments, Explorer with its folder, Store app by id, Squirrel launcher),
    ///   3. new windows are matched while they appear and placed, a second time 3 s later
    ///      (apps that move themselves after the start, DPI change between monitors),
    ///   4. a matched window that closes again (splash screen) frees its entry for the real window.
    /// Apps that can not be started are only placed, when they are open.
    /// </summary>
    internal static class LayoutRestorer
    {
        private const int PollMs = 500;
        private static readonly TimeSpan ReapplyAfter = TimeSpan.FromSeconds(3);

        private sealed class Slot
        {
            public Slot(LayoutWindow entry) => Entry = entry;
            public LayoutWindow Entry { get; }
            public IntPtr Window { get; set; }
            public TimeSpan PlacedAt { get; set; }
            public bool Reapplied { get; set; }
            public bool LaunchDone { get; set; }
            public bool Failed { get; set; }
            public bool Matched => Window != IntPtr.Zero;
        }

        /// <param name="atLogin">Longer waiting (apps of the Windows autostart appear late).</param>
        public static Task<RestoreReport> RestoreAsync(DesktopLayout layout, bool atLogin, CancellationToken ct = default) =>
            StaRunner.Run(() => Native.PerMonitorDpi(() => Restore(layout, atLogin, ct)));

        private static RestoreReport Restore(DesktopLayout layout, bool atLogin, CancellationToken ct)
        {
            var report = new RestoreReport();
            var mapper = new MonitorMapper(layout.Monitors, WindowCapture.GetMonitors());
            var slots = layout.Windows.Where(w => w.Include).Select(w => new Slot(w)).ToList();
            if (slots.Count == 0)
                return report;

            var timeout = TimeSpan.FromSeconds((atLogin ? 120 : 45) + slots.Max(s => Math.Max(0, s.Entry.DelaySeconds)));
            var giveUpOnNotRunning = TimeSpan.FromSeconds(atLogin ? 60 : 4);
            bool wantFolders = slots.Any(s => s.Entry.Launch == LaunchKind.Explorer || s.Entry.ClassName == "CabinetWClass");

            var launchedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            // Several windows with the same start (browser windows with a profile argument, two windows
            // of one app) come from one process: they are started once, like an app without arguments.
            var sharedKeys = slots.Where(s => s.Entry.Launch != LaunchKind.Explorer)
                                  .GroupBy(s => LaunchKey(s.Entry), StringComparer.OrdinalIgnoreCase)
                                  .Where(g => g.Count() > 1)
                                  .Select(g => g.Key)
                                  .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var clock = Stopwatch.StartNew();
            var monitors = WindowCapture.GetMonitors();

            while (!ct.IsCancellationRequested)
            {
                var live = WindowCapture.GetWindows(wantFolders, monitors);

                // A matched window that is gone again (splash screen, launcher) frees its slot.
                foreach (var s in slots.Where(s => s.Matched && !Native.IsWindow(s.Window)))
                {
                    s.Window = IntPtr.Zero;
                    s.Reapplied = false;
                    report.Placed--;
                }

                // Match and place
                var claimed = new HashSet<IntPtr>(slots.Where(s => s.Matched).Select(s => s.Window));
                foreach (var s in slots.Where(s => !s.Matched && !s.Failed))
                {
                    var match = BestMatch(s.Entry, live, claimed);
                    if (match == null)
                        continue;

                    claimed.Add(match.Handle);
                    s.Window = match.Handle;
                    s.PlacedAt = clock.Elapsed;
                    if (Apply(match.Handle, s.Entry, mapper))
                        report.Placed++;
                    else
                    {
                        s.Failed = true;
                        report.Failed.Add(s.Entry.AppName);
                    }
                }

                // Second placement for windows that moved themselves after the start
                foreach (var s in slots.Where(s => s.Matched && !s.Failed && !s.Reapplied && clock.Elapsed - s.PlacedAt >= ReapplyAfter))
                {
                    s.Reapplied = true;
                    Apply(s.Window, s.Entry, mapper);
                }

                // Start what is missing
                foreach (var s in slots.Where(s => !s.Matched && !s.LaunchDone && s.Entry.Start == StartMode.Auto && s.Entry.CanStart))
                {
                    if (clock.Elapsed < TimeSpan.FromSeconds(Math.Max(0, s.Entry.DelaySeconds)))
                        continue;

                    s.LaunchDone = true;
                    var key = LaunchKey(s.Entry);

                    // An app without arguments (browser with its own session restore, single instance apps)
                    // is started once and not at all when it runs already: one process brings its windows.
                    // Explorer folders and apps with a file or arguments get a start per window.
                    bool group = IsGroupLaunch(s.Entry) || sharedKeys.Contains(key);
                    if (group && (launchedKeys.Contains(key) || live.Any(w => SameApp(s.Entry, w))))
                        continue;

                    launchedKeys.Add(key);
                    if (Launch(s.Entry, out string? error))
                        report.Started++;
                    else
                    {
                        s.Failed = true;
                        report.NotFound.Add(s.Entry.AppName + (error != null ? $" ({error})" : ""));
                    }
                }

                // Done?
                bool allPlaced = slots.All(s => s.Failed || (s.Matched && s.Reapplied));
                bool onlyNotRunningLeft = slots.All(s =>
                    s.Failed ||
                    (s.Matched && s.Reapplied) ||
                    (!s.Matched && (s.Entry.Start == StartMode.PositionOnly || !s.Entry.CanStart) && clock.Elapsed > giveUpOnNotRunning));
                if (allPlaced || onlyNotRunningLeft || clock.Elapsed > timeout)
                    break;

                Thread.Sleep(PollMs);
            }

            foreach (var s in slots.Where(s => !s.Matched && !s.Failed))
                report.NotFound.Add(s.Entry.AppName);

            return report;
        }

        // ------------------------------------------------------------------
        // Matching
        // ------------------------------------------------------------------

        private static LiveWindow? BestMatch(LayoutWindow entry, List<LiveWindow> live, HashSet<IntPtr> claimed)
        {
            LiveWindow? best = null;
            int bestScore = 0;

            foreach (var w in live)
            {
                if (claimed.Contains(w.Handle))
                    continue;
                int score = Score(entry, w);
                if (score > bestScore)
                {
                    best = w;
                    bestScore = score;
                }
            }
            return best;
        }

        /// <summary>0 = not this app. Higher = more likely the same window.</summary>
        private static int Score(LayoutWindow e, LiveWindow w)
        {
            int score;

            if (e.Launch == LaunchKind.Explorer || e.ClassName == "CabinetWClass")
            {
                if (!w.IsExplorer)
                    return 0;
                if (e.Launch == LaunchKind.Explorer)
                {
                    // An Explorer window is only "the same" with the same folder; any other would be wrong.
                    if (!w.Folders.Contains(e.StartWith, StringComparer.OrdinalIgnoreCase))
                        return 0;
                    score = 100;
                }
                else
                {
                    score = 10;
                }
            }
            else if (!SameApp(e, w))
            {
                return 0;
            }
            else
            {
                score = 10;
            }

            if (string.Equals(e.Title, w.Title, StringComparison.Ordinal))
                score += 100;
            else if (DocumentPart(e.Title) is { Length: > 0 } doc &&
                     string.Equals(doc, DocumentPart(w.Title), StringComparison.OrdinalIgnoreCase))
                score += 50;

            if (string.Equals(e.ClassName, w.ClassName, StringComparison.Ordinal))
                score += 10;

            if (e.Launch == LaunchKind.Exe && e.StartWith.Length > 0)
            {
                var args = WindowCapture.ArgumentsOf(w.CommandLine, w.ExePath);
                if (string.Equals(args, e.StartWith, StringComparison.OrdinalIgnoreCase))
                    score += 80;
            }

            return score;
        }

        private static bool SameApp(LayoutWindow e, LiveWindow w)
        {
            if (e.Launch == LaunchKind.PackagedApp)
                return string.Equals(e.StartWith, w.AppId, StringComparison.OrdinalIgnoreCase);

            if (string.Equals(e.ExePath, w.ExePath, StringComparison.OrdinalIgnoreCase))
                return true;

            // Updated app in a new version folder (Squirrel, portable apps): same file name is enough.
            return e.ExePath.Length > 0 && w.ExePath.Length > 0 &&
                   string.Equals(Path.GetFileName(e.ExePath), Path.GetFileName(w.ExePath), StringComparison.OrdinalIgnoreCase) &&
                   (e.Launch == LaunchKind.Squirrel || !File.Exists(e.ExePath));
        }

        /// <summary>"Report.docx - Word" -> "Report.docx", "Inbox - Outlook" -> "Inbox".</summary>
        private static string DocumentPart(string title)
        {
            foreach (var sep in new[] { " - ", " – ", " — ", " | " })
            {
                int i = title.LastIndexOf(sep, StringComparison.Ordinal);
                if (i > 0)
                    return title[..i].Trim();
            }
            return title.Trim();
        }

        // ------------------------------------------------------------------
        // Starting
        // ------------------------------------------------------------------

        /// <summary>One start serves all windows of the same key.</summary>
        private static string LaunchKey(LayoutWindow e) => e.Launch switch
        {
            LaunchKind.Exe => e.ExePath + "|" + e.StartWith,
            LaunchKind.Explorer => "explorer|" + e.StartWith,
            LaunchKind.PackagedApp => "app|" + e.StartWith,
            LaunchKind.Squirrel => "squirrel|" + e.StartWith,
            _ => "none|" + e.ExePath
        };

        /// <summary>Starts without anything window specific: one process brings all its windows (or the app is single instance).</summary>
        private static bool IsGroupLaunch(LayoutWindow e) =>
            e.Launch is LaunchKind.PackagedApp or LaunchKind.Squirrel ||
            (e.Launch == LaunchKind.Exe && string.IsNullOrWhiteSpace(e.StartWith));

        private static bool Launch(LayoutWindow e, out string? error)
        {
            error = null;
            try
            {
                switch (e.Launch)
                {
                    case LaunchKind.Exe:
                        if (!File.Exists(e.ExePath))
                        {
                            error = "program not found";
                            return false;
                        }
                        Process.Start(new ProcessStartInfo(e.ExePath, e.StartWith)
                        {
                            UseShellExecute = true,
                            WorkingDirectory = Path.GetDirectoryName(e.ExePath) ?? ""
                        });
                        return true;

                    case LaunchKind.Explorer:
                        var folder = Environment.ExpandEnvironmentVariables(e.StartWith);
                        if (!folder.StartsWith("::", StringComparison.Ordinal) &&
                            !folder.StartsWith("shell:", StringComparison.OrdinalIgnoreCase) &&
                            !Directory.Exists(folder))
                        {
                            error = "folder not found";
                            return false;
                        }
                        Process.Start(new ProcessStartInfo("explorer.exe", $"\"{folder}\"") { UseShellExecute = true });
                        return true;

                    case LaunchKind.PackagedApp:
                        Process.Start(new ProcessStartInfo("explorer.exe", "shell:AppsFolder\\" + e.StartWith) { UseShellExecute = true });
                        return true;

                    case LaunchKind.Squirrel:
                        var root = FindSquirrelRoot(e.ExePath);
                        if (root == null)
                        {
                            error = "Update.exe not found";
                            return false;
                        }
                        Process.Start(new ProcessStartInfo(Path.Combine(root, "Update.exe"), $"--processStart \"{e.StartWith}\"")
                        {
                            UseShellExecute = true,
                            WorkingDirectory = root
                        });
                        return true;

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message; // includes "cancelled by the user" of a UAC prompt
                return false;
            }
        }

        private static string? FindSquirrelRoot(string exePath)
        {
            try
            {
                var root = Path.GetDirectoryName(Path.GetDirectoryName(exePath));
                return root != null && File.Exists(Path.Combine(root, "Update.exe")) ? root : null;
            }
            catch
            {
                return null;
            }
        }

        // ------------------------------------------------------------------
        // Placing
        // ------------------------------------------------------------------

        private static bool Apply(IntPtr h, LayoutWindow e, MonitorMapper mapper)
        {
            if (!Native.IsWindow(h))
                return false;

            // Same monitors as at save time: maximized / minimized windows go back with their own placement.
            if (mapper.Identical && e.Placement != null && e.State != ShowState.Normal)
            {
                var wp = new Native.WINDOWPLACEMENT
                {
                    length = System.Runtime.InteropServices.Marshal.SizeOf<Native.WINDOWPLACEMENT>(),
                    showCmd = e.State == ShowState.Maximized ? Native.SW_SHOWMAXIMIZED : Native.SW_SHOWMINNOACTIVE,
                    rcNormalPosition = Native.RECT.From(e.Placement.ToRectangle())
                };
                if (Native.SetWindowPlacement(h, ref wp))
                    return true;
            }

            var target = mapper.MapBounds(e);

            if (Native.IsZoomed(h))
                Native.ShowWindow(h, Native.SW_RESTORE);
            else if (Native.IsIconic(h))
                Native.ShowWindow(h, Native.SW_SHOWNOACTIVATE);

            const uint flags = Native.SWP_NOZORDER | Native.SWP_NOACTIVATE | Native.SWP_NOOWNERZORDER;
            bool ok = Native.SetWindowPos(h, IntPtr.Zero, target.X, target.Y, target.Width, target.Height, flags);
            // A move to a monitor with another scaling makes the window resize itself: set it again.
            Native.SetWindowPos(h, IntPtr.Zero, target.X, target.Y, target.Width, target.Height, flags);

            if (e.State == ShowState.Maximized)
                Native.ShowWindow(h, Native.SW_MAXIMIZE);
            else if (e.State == ShowState.Minimized)
                Native.ShowWindow(h, Native.SW_SHOWMINNOACTIVE);

            // false: the window belongs to a program running as administrator (Windows blocks that)
            return ok;
        }
    }
}
