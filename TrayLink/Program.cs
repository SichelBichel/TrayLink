using System.Reflection;
using TrayLink.Core;
using TrayLink.Core.Layouts;

namespace TrayLink
{
    internal static class Program
    {
        private const string MutexName = @"Local\ReHoGa.TrayLink.SingleInstance";
        private const string ShowEventName = @"Local\ReHoGa.TrayLink.ShowPopup";

        public static Icon AppIcon { get; private set; } = SystemIcons.Application;

        private static NotifyIcon? _trayIcon;
        private static TrayPopupForm? _popup;
        private static ToolStripMenuItem? _autostartItem;
        private static LayoutManagerForm? _layoutManager;

        [STAThread]
        static int Main(string[] args)
        {
            string startupDirectory = Environment.CurrentDirectory;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // The Explorer context menu starts us in the folder of the clicked item, so the
            // working directory is only a migration source for a normal start.
            AppPaths.MigrateLegacyConfig(args.Length == 0 ? startupDirectory : AppContext.BaseDirectory);

            // Command line modes (Explorer context menu etc.) do their job and exit.
            if (args.Length > 0)
            {
                int? exitCode = HandleCommandLine(args);
                if (exitCode.HasValue)
                    return exitCode.Value;
            }

            using var mutex = new Mutex(true, MutexName, out bool isFirstInstance);
            if (!isFirstInstance)
            {
                // Already running: ask the running instance to open its popup.
                try
                {
                    using var evt = EventWaitHandle.OpenExisting(ShowEventName);
                    evt.Set();
                }
                catch
                {
                    // ignore
                }
                return 0;
            }

            AppIcon = LoadIconFromResources("TrayLink.Resources.TrayLink.ico");
            AppSettings.Load();
            ShellIntegration.RepairIfStale();
            Autostart.RepairIfStale();

            _popup = new TrayPopupForm();
            // Force handle creation so BeginInvoke works before the popup was shown once.
            _ = _popup.Handle;

            _trayIcon = new NotifyIcon
            {
                Icon = AppIcon,
                Visible = true,
                Text = "TrayLink",
                ContextMenuStrip = BuildTrayMenu()
            };
            _trayIcon.MouseClick += TrayIcon_MouseClick;

            Application.ApplicationExit += (s, e) =>
            {
                // Without this the icon stays in the tray as a "ghost" until the mouse hovers over it.
                if (_trayIcon != null)
                {
                    _trayIcon.Visible = false;
                    _trayIcon.Dispose();
                }
            };

            StartShowEventListener();
            LayoutUi.ScheduleAutoRestore();

            Application.Run();
            return 0;
        }

        private static int? HandleCommandLine(string[] args)
        {
            switch (args[0].ToLowerInvariant())
            {
                case "--add":
                    var paths = args.Skip(1).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                    var valid = paths.Where(p => File.Exists(p) || Directory.Exists(p)).ToList();
                    if (valid.Count == 0)
                    {
                        MessageBox.Show("Path does not exist:\n" + string.Join("\n", paths), "TrayLink",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return 1;
                    }
                    try
                    {
                        // A running instance picks the change up through its file watcher.
                        ActionStore.Append(valid.Select(ActionConfig.FromPath));
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not add entry:\n" + ex.Message, "TrayLink",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 2;
                    }

                case "--register":
                    try { ShellIntegration.Register(); return 0; }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "TrayLink", MessageBoxButtons.OK, MessageBoxIcon.Error); return 2; }

                case "--unregister":
                    try { ShellIntegration.Unregister(); return 0; }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "TrayLink", MessageBoxButtons.OK, MessageBoxIcon.Error); return 2; }

                default:
                    // Unknown arguments (for example from an updater restart): start normally.
                    return null;
            }
        }

        private static ContextMenuStrip BuildTrayMenu()
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("Open TrayLink", null, (s, e) => ShowPopup());
            menu.Items.Add(BuildLayoutMenu());
            menu.Items.Add(new ToolStripSeparator());

            _autostartItem = new ToolStripMenuItem("Start with Windows") { CheckOnClick = true };
            _autostartItem.Click += (s, e) =>
            {
                try
                {
                    Autostart.SetEnabled(_autostartItem.Checked);
                }
                catch (Exception ex)
                {
                    _autostartItem.Checked = Autostart.IsEnabled;
                    MessageBox.Show(ex.Message, "TrayLink", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            menu.Items.Add(_autostartItem);

            menu.Items.Add("Check for updates", null, async (s, e) =>
            {
                ShowPopup();
                if (_popup != null)
                    await _popup.RunUpdateCheckAsync();
            });

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Exit", null, (s, e) => Application.Exit());

            menu.Opening += (s, e) => _autostartItem.Checked = Autostart.IsEnabled;
            return menu;
        }

        private static ToolStripMenuItem BuildLayoutMenu()
        {
            var root = new ToolStripMenuItem("Desktop layouts");
            // Filled when it opens, so it always lists the saved layouts.
            root.DropDownItems.Add(new ToolStripMenuItem("(loading)") { Enabled = false });
            root.DropDownOpening += (s, e) =>
            {
                root.DropDownItems.Clear();
                root.DropDownItems.Add("Save current desktop...", null, async (s2, e2) => await LayoutUi.SaveCurrentAsync(null));

                var layouts = LayoutStore.LoadAll();
                if (layouts.Count > 0)
                {
                    root.DropDownItems.Add(new ToolStripSeparator());
                    foreach (var layout in layouts)
                    {
                        var item = new ToolStripMenuItem("Restore \"" + layout.Name + "\"") { Enabled = !LayoutUi.Busy };
                        if (string.Equals(AppSettings.Current.AutoRestoreLayout, layout.Name, StringComparison.CurrentCultureIgnoreCase))
                            item.Text += "  (at Windows start)";
                        var captured = layout;
                        item.Click += async (s2, e2) => await LayoutUi.RestoreAsync(captured, atLogin: false);
                        root.DropDownItems.Add(item);
                    }
                }

                root.DropDownItems.Add(new ToolStripSeparator());
                root.DropDownItems.Add("Manage layouts...", null, (s2, e2) => ShowLayoutManager());
            };
            return root;
        }

        public static void ShowLayoutManager()
        {
            if (_layoutManager == null || _layoutManager.IsDisposed)
            {
                _layoutManager = new LayoutManagerForm();
                _layoutManager.FormClosed += (s, e) => _layoutManager = null;
                _layoutManager.Show();
            }
            else
            {
                if (_layoutManager.WindowState == FormWindowState.Minimized)
                    _layoutManager.WindowState = FormWindowState.Normal;
                _layoutManager.BringToFront();
                _layoutManager.Activate();
            }
        }

        /// <summary>Balloon of the tray icon (layout saved / restored).</summary>
        public static void ShowInfo(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (_trayIcon == null)
                return;
            if (text.Length > 250)
                text = text[..247] + "...";
            _trayIcon.ShowBalloonTip(8000, title, text, icon);
        }

        private static void StartShowEventListener()
        {
            var evt = new EventWaitHandle(false, EventResetMode.AutoReset, ShowEventName);
            var thread = new Thread(() =>
            {
                while (true)
                {
                    evt.WaitOne();
                    try
                    {
                        _popup?.BeginInvoke(new Action(ShowPopup));
                    }
                    catch
                    {
                        return; // app is shutting down
                    }
                }
            })
            {
                IsBackground = true,
                Name = "TrayLink show listener"
            };
            thread.Start();
        }

        private static Icon LoadIconFromResources(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
                return new Icon(stream);
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        private static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                TogglePopup();
        }

        private static void TogglePopup()
        {
            if (_popup == null)
                return;

            if (_popup.Visible)
            {
                _popup.Hide();
                return;
            }

            // Clicking the tray icon deactivates the popup first (auto hide), then the click arrives.
            // Without this check the popup would immediately reopen instead of closing.
            if (_popup.WasAutoHiddenJustNow)
                return;

            ShowPopup();
        }

        public static void ShowPopup()
        {
            if (_popup == null)
                return;

            if (!_popup.Visible)
            {
                _popup.PlaceNearTray(Cursor.Position);
                _popup.Show();
            }

            _popup.BringToFront();
            _popup.Activate();
        }
    }
}
