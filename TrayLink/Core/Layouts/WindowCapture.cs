using System.Text.RegularExpressions;

namespace TrayLink.Core.Layouts
{
    /// <summary>A top level window as it is open right now. Coordinates in physical pixels.</summary>
    internal sealed class LiveWindow
    {
        public IntPtr Handle { get; init; }
        public uint ProcessId { get; init; }
        public string ExePath { get; init; } = "";
        public string Title { get; init; } = "";
        public string ClassName { get; init; } = "";
        public ShowState State { get; init; }

        /// <summary>Normal rectangle in screen coordinates (for a maximized or minimized window an approximation).</summary>
        public Rectangle Bounds { get; init; }

        /// <summary>Raw normal rectangle of GetWindowPlacement (workspace coordinates).</summary>
        public Rectangle PlacementNormal { get; init; }

        public string? AppId { get; init; }
        public List<string> Folders { get; set; } = new();

        public bool IsExplorer => ClassName == "CabinetWClass";
        public string FileName => Path.GetFileName(ExePath);

        private string? _commandLine;
        private bool _commandLineRead;

        public string? CommandLine
        {
            get
            {
                if (!_commandLineRead)
                {
                    _commandLine = Native.GetProcessCommandLine(ProcessId);
                    _commandLineRead = true;
                }
                return _commandLine;
            }
        }
    }

    internal static class WindowCapture
    {
        private static readonly HashSet<string> SkippedClasses = new(StringComparer.Ordinal)
        {
            "Progman", "WorkerW", "Shell_TrayWnd", "Shell_SecondaryTrayWnd", "Windows.UI.Core.CoreWindow",
            "NotifyIconOverflowWindow", "TopLevelWindowForOverflowXamlIsland", "XamlExplorerHostIslandWindow"
        };

        private static readonly HashSet<string> SkippedProcesses = new(StringComparer.OrdinalIgnoreCase)
        {
            "ShellExperienceHost.exe", "StartMenuExperienceHost.exe", "SearchHost.exe", "SearchApp.exe",
            "TextInputHost.exe", "LockApp.exe", "ShellHost.exe", "Widgets.exe", "WidgetBoard.exe"
        };

        private static readonly Regex SquirrelFolder = new(@"\\app-\d[\d.]*\\[^\\]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>All monitors, numbered from left to right. Call inside <see cref="Native.PerMonitorDpi"/>.</summary>
        public static List<MonitorSnapshot> GetMonitors()
        {
            var list = new List<MonitorSnapshot>();
            Native.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr h, IntPtr hdc, ref Native.RECT r, IntPtr d) =>
            {
                var info = new Native.MONITORINFOEX { cbSize = System.Runtime.InteropServices.Marshal.SizeOf<Native.MONITORINFOEX>() };
                if (Native.GetMonitorInfo(h, ref info))
                {
                    list.Add(new MonitorSnapshot
                    {
                        DeviceName = info.szDevice ?? "",
                        Primary = (info.dwFlags & Native.MONITORINFOF_PRIMARY) != 0,
                        Bounds = Box.From(info.rcMonitor.ToRectangle()),
                        WorkArea = Box.From(info.rcWork.ToRectangle())
                    });
                }
                return true;
            }, IntPtr.Zero);

            list = list.OrderBy(m => m.Bounds.X).ThenBy(m => m.Bounds.Y).ToList();
            for (int i = 0; i < list.Count; i++)
                list[i].Index = i;
            return list;
        }

        /// <summary>All normal application windows. Call inside <see cref="Native.PerMonitorDpi"/>, on an STA thread when folders are wanted.</summary>
        public static List<LiveWindow> GetWindows(bool withExplorerFolders, List<MonitorSnapshot> monitors)
        {
            var handles = new List<IntPtr>();
            Native.EnumWindows((h, l) => { handles.Add(h); return true; }, IntPtr.Zero);

            uint ownPid = (uint)Environment.ProcessId;
            var primary = monitors.FirstOrDefault(m => m.Primary) ?? monitors.FirstOrDefault();
            var workspaceOffset = primary == null
                ? Point.Empty
                : new Point(primary.WorkArea.X - primary.Bounds.X, primary.WorkArea.Y - primary.Bounds.Y);

            var result = new List<LiveWindow>();
            foreach (var h in handles)
            {
                try
                {
                    var w = Inspect(h, ownPid, workspaceOffset);
                    if (w != null)
                        result.Add(w);
                }
                catch
                {
                    // window closed meanwhile
                }
            }

            if (withExplorerFolders && result.Any(w => w.IsExplorer))
            {
                var folders = ExplorerWindows.GetFolders();
                foreach (var w in result.Where(w => w.IsExplorer))
                    if (folders.TryGetValue(w.Handle, out var list))
                        w.Folders = list;
            }

            return result;
        }

        private static LiveWindow? Inspect(IntPtr h, uint ownPid, Point workspaceOffset)
        {
            if (!Native.IsWindowVisible(h) || Native.IsCloaked(h))
                return null;
            if (Native.GetWindow(h, Native.GW_OWNER) != IntPtr.Zero)
                return null; // dialogs and popups belong to their owner

            long ex = Native.GetWindowLongPtr(h, Native.GWL_EXSTYLE).ToInt64();
            bool appWindow = (ex & Native.WS_EX_APPWINDOW) != 0;
            if (!appWindow && ((ex & Native.WS_EX_TOOLWINDOW) != 0 || (ex & Native.WS_EX_NOACTIVATE) != 0))
                return null;

            string cls = Native.GetClass(h);
            if (SkippedClasses.Contains(cls))
                return null;

            string title = Native.GetText(h);
            if (string.IsNullOrWhiteSpace(title))
                return null;

            Native.GetWindowThreadProcessId(h, out uint pid);
            if (pid == 0 || pid == ownPid)
                return null;

            string exe = Native.GetProcessPath(pid) ?? "";
            string? appId = null;

            // Store apps: the frame window belongs to ApplicationFrameHost, the app itself is a child window.
            if (exe.EndsWith("ApplicationFrameHost.exe", StringComparison.OrdinalIgnoreCase))
            {
                uint appPid = FindHostedAppProcess(h, pid);
                if (appPid != 0)
                {
                    exe = Native.GetProcessPath(appPid) ?? exe;
                    appId = Native.GetPackagedAppId(appPid);
                }
            }
            else
            {
                appId = Native.GetPackagedAppId(pid);
            }

            string file = Path.GetFileName(exe);
            if (SkippedProcesses.Contains(file))
                return null;
            if (file.Equals("explorer.exe", StringComparison.OrdinalIgnoreCase) && cls != "CabinetWClass")
                return null; // desktop, taskbar and other shell windows

            var placement = new Native.WINDOWPLACEMENT { length = System.Runtime.InteropServices.Marshal.SizeOf<Native.WINDOWPLACEMENT>() };
            Native.GetWindowPlacement(h, ref placement);
            var normal = placement.rcNormalPosition.ToRectangle();

            ShowState state = Native.IsIconic(h) ? ShowState.Minimized
                            : Native.IsZoomed(h) ? ShowState.Maximized
                            : ShowState.Normal;

            Rectangle bounds;
            if (state == ShowState.Normal)
            {
                // GetWindowRect and not the placement: a snapped window (half screen) keeps its old
                // size in the placement, the rectangle is what you see.
                Native.GetWindowRect(h, out var r);
                bounds = r.ToRectangle();
                if (bounds.Width < 40 || bounds.Height < 40)
                    return null;
            }
            else
            {
                bounds = new Rectangle(normal.X + workspaceOffset.X, normal.Y + workspaceOffset.Y, normal.Width, normal.Height);
            }

            return new LiveWindow
            {
                Handle = h,
                ProcessId = pid,
                ExePath = exe,
                Title = title,
                ClassName = cls,
                State = state,
                Bounds = bounds,
                PlacementNormal = normal,
                AppId = appId
            };
        }

        private static uint FindHostedAppProcess(IntPtr frame, uint framePid)
        {
            uint found = 0;
            Native.EnumChildWindows(frame, (child, l) =>
            {
                Native.GetWindowThreadProcessId(child, out uint pid);
                if (pid != 0 && pid != framePid)
                {
                    found = pid;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return found;
        }

        // ------------------------------------------------------------------
        // Snapshot
        // ------------------------------------------------------------------

        /// <summary>Takes the snapshot of the desktop. Runs on an STA thread (see <see cref="StaRunner"/>).</summary>
        public static DesktopLayout Capture(string name)
        {
            return Native.PerMonitorDpi(() =>
            {
                var monitors = GetMonitors();
                var windows = GetWindows(withExplorerFolders: true, monitors);

                var layout = new DesktopLayout { Name = name, SavedAt = DateTime.Now, Monitors = monitors };
                foreach (var w in windows)
                    layout.Windows.Add(ToLayoutWindow(w, monitors));
                return layout;
            });
        }

        public static LayoutWindow ToLayoutWindow(LiveWindow w, List<MonitorSnapshot> monitors)
        {
            var entry = new LayoutWindow
            {
                ExePath = w.ExePath,
                ProcessName = Path.GetFileNameWithoutExtension(w.ExePath),
                Title = w.Title,
                ClassName = w.ClassName,
                State = w.State,
                Bounds = Box.From(w.Bounds),
                Placement = Box.From(w.PlacementNormal),
                MonitorIndex = MonitorIndexOf(w.Bounds, monitors)
            };

            if (w.IsExplorer)
            {
                entry.ProcessName = "File Explorer";
                if (w.Folders.Count > 0)
                {
                    entry.Launch = LaunchKind.Explorer;
                    entry.StartWith = w.Folders[0];
                    entry.ExtraPaths = w.Folders.Skip(1).ToList();
                }
                else
                {
                    entry.Launch = LaunchKind.None;
                }
            }
            else if (!string.IsNullOrEmpty(w.AppId))
            {
                entry.Launch = LaunchKind.PackagedApp;
                entry.StartWith = w.AppId!;
            }
            else if (IsSquirrel(w.ExePath))
            {
                entry.Launch = LaunchKind.Squirrel;
                entry.StartWith = Path.GetFileName(w.ExePath);
            }
            else if (!string.IsNullOrEmpty(w.ExePath) && File.Exists(w.ExePath))
            {
                entry.Launch = LaunchKind.Exe;
                entry.StartWith = ArgumentsOf(w.CommandLine, w.ExePath);
            }
            else
            {
                entry.Launch = LaunchKind.None; // no access to the process (other user, protected)
            }

            if (!entry.CanStart)
                entry.Start = StartMode.PositionOnly;

            return entry;
        }

        /// <summary>Discord, Slack, GitHub Desktop ...: %LocalAppData%\App\app-1.2.3\App.exe with Update.exe one level up.</summary>
        public static bool IsSquirrel(string exePath)
        {
            if (string.IsNullOrEmpty(exePath) || !SquirrelFolder.IsMatch(exePath))
                return false;
            var root = Path.GetDirectoryName(Path.GetDirectoryName(exePath));
            return root != null && File.Exists(Path.Combine(root, "Update.exe"));
        }

        /// <summary>The arguments of a command line without the program part.</summary>
        public static string ArgumentsOf(string? commandLine, string exePath)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                return "";

            var s = commandLine.TrimStart();
            if (s.StartsWith('"'))
            {
                int end = s.IndexOf('"', 1);
                return end < 0 ? "" : s[(end + 1)..].Trim();
            }

            if (s.StartsWith(exePath, StringComparison.OrdinalIgnoreCase))
                return s[exePath.Length..].Trim();

            int space = s.IndexOf(' ');
            return space < 0 ? "" : s[(space + 1)..].Trim();
        }

        public static int MonitorIndexOf(Rectangle bounds, List<MonitorSnapshot> monitors)
        {
            if (monitors.Count == 0)
                return 0;

            var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            var hit = monitors.FirstOrDefault(m => m.Bounds.ToRectangle().Contains(center));
            if (hit != null)
                return hit.Index;

            // Center outside of every monitor: the one with the largest overlap, else the nearest.
            var best = monitors
                .Select(m => (m, area: Overlap(m.Bounds.ToRectangle(), bounds)))
                .OrderByDescending(x => x.area)
                .First();
            if (best.area > 0)
                return best.m.Index;

            return monitors.OrderBy(m => Distance(m.Bounds.ToRectangle(), center)).First().Index;
        }

        private static long Overlap(Rectangle a, Rectangle b)
        {
            var i = Rectangle.Intersect(a, b);
            return i.IsEmpty ? 0 : (long)i.Width * i.Height;
        }

        private static double Distance(Rectangle r, Point p)
        {
            int dx = Math.Max(Math.Max(r.Left - p.X, 0), p.X - r.Right);
            int dy = Math.Max(Math.Max(r.Top - p.Y, 0), p.Y - r.Bottom);
            return Math.Sqrt((double)dx * dx + (double)dy * dy);
        }
    }
}
