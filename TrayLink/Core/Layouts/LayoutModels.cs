using System.Text.Json.Serialization;

namespace TrayLink.Core.Layouts
{
    /// <summary>A saved desktop: the monitors at save time and every window on them.</summary>
    public sealed class DesktopLayout
    {
        public int FormatVersion { get; set; } = 1;
        public string Name { get; set; } = "Layout";
        public DateTime SavedAt { get; set; } = DateTime.Now;
        public List<MonitorSnapshot> Monitors { get; set; } = new();
        public List<LayoutWindow> Windows { get; set; } = new();
    }

    public sealed class MonitorSnapshot
    {
        public int Index { get; set; }
        public string DeviceName { get; set; } = "";
        public bool Primary { get; set; }
        public Box Bounds { get; set; } = new();
        public Box WorkArea { get; set; } = new();

        public override string ToString() => $"{Index + 1} ({Bounds.Width} x {Bounds.Height})";
    }

    /// <summary>A rectangle in physical screen pixels (JSON friendly).</summary>
    public sealed class Box
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static Box From(Rectangle r) => new() { X = r.X, Y = r.Y, Width = r.Width, Height = r.Height };
        public Rectangle ToRectangle() => new(X, Y, Width, Height);
    }

    public enum ShowState
    {
        Normal,
        Maximized,
        Minimized
    }

    public enum StartMode
    {
        /// <summary>Start the app when no matching window is open, then place it.</summary>
        Auto,
        /// <summary>Never start, only place the window when it is open already.</summary>
        PositionOnly
    }

    public enum LaunchKind
    {
        /// <summary>No way to start it (system window, exe gone): position only.</summary>
        None,
        /// <summary>Start the exe with the saved arguments.</summary>
        Exe,
        /// <summary>File Explorer window with a folder.</summary>
        Explorer,
        /// <summary>Packaged app (Store, Windows Terminal ...), started with its app id.</summary>
        PackagedApp,
        /// <summary>Squirrel installed app (Discord, Slack ...): exe folder changes with every update.</summary>
        Squirrel
    }

    public sealed class LayoutWindow
    {
        public bool Include { get; set; } = true;

        // Identity of the window at save time
        public string ExePath { get; set; } = "";
        public string ProcessName { get; set; } = "";
        public string Title { get; set; } = "";
        public string ClassName { get; set; } = "";

        // How to bring it back
        public LaunchKind Launch { get; set; } = LaunchKind.Exe;
        public StartMode Start { get; set; } = StartMode.Auto;

        /// <summary>Arguments for Exe, folder for Explorer, app id for PackagedApp, exe name for Squirrel.</summary>
        public string StartWith { get; set; } = "";

        /// <summary>Other folders of the window (Explorer tabs), informative.</summary>
        public List<string> ExtraPaths { get; set; } = new();

        /// <summary>Seconds to wait after the restore started before this app is started.</summary>
        public int DelaySeconds { get; set; }

        // Where
        public int MonitorIndex { get; set; }
        public ShowState State { get; set; } = ShowState.Normal;

        /// <summary>Normal (not maximized) rectangle in physical screen pixels.</summary>
        public Box Bounds { get; set; } = new();

        /// <summary>
        /// Raw normal rectangle of the window placement (workspace coordinates). Used for maximized and
        /// minimized windows when the monitors are the same as at save time.
        /// </summary>
        public Box? Placement { get; set; }

        [JsonIgnore]
        public string AppName => string.IsNullOrEmpty(ProcessName) ? Path.GetFileNameWithoutExtension(ExePath) : ProcessName;

        [JsonIgnore]
        public bool CanStart => Launch != LaunchKind.None;
    }

    /// <summary>What a restore did, shown to the user at the end.</summary>
    public sealed class RestoreReport
    {
        public int Placed { get; set; }
        public int Started { get; set; }
        public List<string> NotFound { get; } = new();
        public List<string> Failed { get; } = new();

        public string Summary
        {
            get
            {
                var text = $"{Placed} window(s) placed, {Started} app(s) started.";
                if (NotFound.Count > 0)
                    text += $"\nNot found: {string.Join(", ", NotFound.Distinct().Take(6))}{(NotFound.Distinct().Count() > 6 ? " ..." : "")}";
                if (Failed.Count > 0)
                    text += $"\nCould not place: {string.Join(", ", Failed.Distinct().Take(6))}";
                return text;
            }
        }
    }
}
