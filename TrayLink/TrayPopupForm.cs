using System.Diagnostics;
using TrayLink.Core;
using TrayLink.Updater;

namespace TrayLink
{
    public partial class TrayPopupForm : Form, ILinkItemHost
    {
        private List<ActionConfig> _actions = new();

        /// <summary>Text of actions.ini as we last read or wrote it. Used to detect external changes.</summary>
        private string _lastKnownText = "";

        private readonly FileSystemWatcher? _watcher;
        private readonly System.Windows.Forms.Timer _reloadDebounce = new() { Interval = 300 };

        private readonly int _designHeight;
        private int _modalDepth;
        private bool _dragging;
        private bool _reloadPending;
        private bool _repopulateQueued;
        private bool _updateRunning;
        private DateTime _autoHiddenAtUtc = DateTime.MinValue;

        public TrayPopupForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = Program.AppIcon;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            KeyPreview = true;
            _designHeight = Height;

            label3.Text = "V " + VersionUtil.StripBuildMeta(VersionUtil.GetInstalledVersion());

            chkAutoHide.Checked = AppSettings.Current.AutoHide;
            chkAutoHide.CheckedChanged += (s, e) =>
            {
                AppSettings.Current.AutoHide = chkAutoHide.Checked;
                AppSettings.Save();
            };

            Deactivate += OnDeactivateAutoHide;
            FormClosing += OnFormClosing;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    Hide();
                    e.Handled = true;
                }
            };

            actionPanel.DragEnter += ActionPanel_DragOver;
            actionPanel.DragOver += ActionPanel_DragOver;
            actionPanel.DragDrop += ActionPanel_DragDrop;
            actionPanel.SizeChanged += (s, e) => UpdateCardWidths();

            _reloadDebounce.Tick += (s, e) =>
            {
                _reloadDebounce.Stop();
                ReloadFromDisk(force: false);
            };

            try
            {
                AppPaths.EnsureDataDir();
                _watcher = new FileSystemWatcher(AppPaths.DataDir, "actions.ini")
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.CreationTime,
                    SynchronizingObject = this
                };
                _watcher.Changed += OnConfigFileChanged;
                _watcher.Created += OnConfigFileChanged;
                _watcher.Renamed += OnConfigFileChanged;
                _watcher.Deleted += OnConfigFileChanged;
                _watcher.EnableRaisingEvents = true;
            }
            catch
            {
                _watcher = null; // auto reload is a convenience, the Reload button still works
            }

            UpdateShellButton();
            ReloadFromDisk(force: true);
        }

        // ------------------------------------------------------------------
        // Popup behaviour
        // ------------------------------------------------------------------

        /// <summary>True right after the popup hid itself because it lost focus (see Program.TogglePopup).</summary>
        public bool WasAutoHiddenJustNow => (DateTime.UtcNow - _autoHiddenAtUtc).TotalMilliseconds < 500;

        /// <summary>Places the popup in the corner next to the taskbar of the screen the cursor is on.</summary>
        public void PlaceNearTray(Point cursor)
        {
            var screen = Screen.FromPoint(cursor);
            var wa = screen.WorkingArea;
            var bounds = screen.Bounds;

            Height = Math.Min(_designHeight, wa.Height - 20);

            int x = wa.Left > bounds.Left ? wa.Left + 10 : wa.Right - Width - 10;  // taskbar left / otherwise right
            int y = wa.Top > bounds.Top ? wa.Top + 10 : wa.Bottom - Height - 10;   // taskbar top / otherwise bottom

            Location = new Point(Math.Max(wa.Left, x), Math.Max(wa.Top, y));
        }

        private void OnDeactivateAutoHide(object? sender, EventArgs e)
        {
            // Editor, message boxes and the updater are owned by the popup. They take the focus,
            // but the popup must stay open behind them.
            if (!chkAutoHide.Checked || _modalDepth > 0 || _dragging)
                return;

            _autoHiddenAtUtc = DateTime.UtcNow;
            Hide();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            // The X button only hides the popup. Exit is in the tray icon menu.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private T RunModal<T>(Func<T> action)
        {
            _modalDepth++;
            try
            {
                return action();
            }
            finally
            {
                _modalDepth--;
            }
        }

        private void ShowError(string title, string message) =>
            RunModal(() => MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error));

        // ------------------------------------------------------------------
        // Loading / saving
        // ------------------------------------------------------------------

        private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            _reloadDebounce.Stop();
            _reloadDebounce.Start();
        }

        private void ReloadFromDisk(bool force)
        {
            if (_dragging || _modalDepth > 0)
            {
                _reloadPending = true;
                return;
            }

            _reloadPending = false;
            try
            {
                if (!force && File.Exists(AppPaths.ActionsFile) &&
                    ActionStore.ReadAllTextShared(AppPaths.ActionsFile) == _lastKnownText)
                    return; // our own save, nothing new

                _actions = ActionStore.Load(out var text);
                _lastKnownText = text;
                RequestRepopulate();
            }
            catch (Exception ex)
            {
                ShowError("Error", $"Error loading configuration:\n{ex.Message}");
            }
        }

        private void RunPendingReload()
        {
            if (_reloadPending && !_dragging && _modalDepth == 0)
                ReloadFromDisk(force: false);
        }

        private void SaveAndRefresh()
        {
            try
            {
                MergeExternalAdditions();
                _lastKnownText = ActionStore.Save(_actions);
            }
            catch (Exception ex)
            {
                ShowError("Error", $"Error saving configuration:\n{ex.Message}");
            }

            _reloadPending = false;
            RequestRepopulate();
        }

        /// <summary>
        /// Entries added from outside (Explorer context menu) while the editor was open would be
        /// overwritten by our save. Keep them by appending them to the in memory list.
        /// </summary>
        private void MergeExternalAdditions()
        {
            if (!File.Exists(AppPaths.ActionsFile))
                return;

            var diskText = ActionStore.ReadAllTextShared(AppPaths.ActionsFile);
            if (diskText == _lastKnownText)
                return;

            var known = ActionStore.Parse(_lastKnownText);
            foreach (var entry in ActionStore.Parse(diskText))
            {
                bool wasKnown = known.Any(k => SameEntry(k, entry));
                bool isPresent = _actions.Any(a => SameEntry(a, entry));
                if (!wasKnown && !isPresent)
                    _actions.Add(entry);
            }
        }

        private static bool SameEntry(ActionConfig a, ActionConfig b) =>
            string.Equals(a.ActionName, b.ActionName, StringComparison.Ordinal) &&
            string.Equals(a.ActionType, b.ActionType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(a.PathOrUrl, b.PathOrUrl, StringComparison.OrdinalIgnoreCase);

        // ------------------------------------------------------------------
        // Cards
        // ------------------------------------------------------------------

        public bool CanReorder => string.IsNullOrWhiteSpace(searchBar.Text);

        private int CardWidth =>
            Math.Max(150, actionPanel.Width - SystemInformation.VerticalScrollBarWidth - 2 * SystemInformation.BorderSize.Width - 8);

        /// <summary>
        /// Rebuilding the cards disposes the button whose click started the change.
        /// Doing that inside the click handler can crash, so it is always queued.
        /// </summary>
        private void RequestRepopulate()
        {
            if (_repopulateQueued)
                return;
            _repopulateQueued = true;

            if (IsHandleCreated)
            {
                BeginInvoke(new Action(() =>
                {
                    _repopulateQueued = false;
                    PopulateCards();
                }));
            }
            else
            {
                _repopulateQueued = false;
                PopulateCards();
            }
        }

        private void PopulateCards()
        {
            var filter = searchBar.Text.Trim();
            int scrollY = -actionPanel.AutoScrollPosition.Y;

            actionPanel.SuspendLayout();
            var old = actionPanel.Controls.Cast<Control>().ToList();
            actionPanel.Controls.Clear();
            foreach (var c in old)
                c.Dispose();

            int width = CardWidth;
            for (int i = 0; i < _actions.Count; i++)
            {
                var action = _actions[i];
                if (!Matches(action, filter))
                    continue;

                actionPanel.Controls.Add(new LinkItem(action, i + 1, this) { Width = width });
            }
            actionPanel.ResumeLayout();

            actionPanel.AutoScrollPosition = new Point(0, scrollY);
        }

        private void UpdateCardWidths()
        {
            int width = CardWidth;
            foreach (Control c in actionPanel.Controls)
                c.Width = width;
        }

        private static bool Matches(ActionConfig a, string filter) =>
            filter.Length == 0 ||
            a.ActionName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
            a.ActionType.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
            a.PathOrUrl.Contains(filter, StringComparison.OrdinalIgnoreCase);

        public void ExecuteAction(ActionConfig config) =>
            RunModal(() => { ActionRunner.Execute(config, this); return 0; });

        public void EditAction(ActionConfig config)
        {
            int index = _actions.IndexOf(config);
            if (index < 0)
                return;

            using var form = new EditActionForm(config, index + 1, _actions.Count, isNew: false);
            if (RunModal(() => form.ShowDialog(this)) != DialogResult.OK)
            {
                RunPendingReload();
                return;
            }

            // The list may have been reloaded meanwhile, so look the entry up again.
            index = _actions.IndexOf(config);
            if (index < 0)
                return;

            _actions[index] = form.Result;
            MoveItem(index, form.Position - 1);
            SaveAndRefresh();
        }

        public void DeleteAction(ActionConfig config)
        {
            var answer = RunModal(() => MessageBox.Show(this, $"Delete '{config.ActionName}'?", "Confirm deletion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question));
            if (answer != DialogResult.Yes)
                return;

            // Remove exactly this entry, not every entry with the same name.
            if (_actions.Remove(config))
                SaveAndRefresh();
        }

        public void MoveAction(ActionConfig config, int delta)
        {
            int index = _actions.IndexOf(config);
            if (index < 0)
                return;

            int target = Math.Clamp(index + delta, 0, _actions.Count - 1);
            if (target == index)
                return;

            MoveItem(index, target);
            SaveAndRefresh();
        }

        private void MoveItem(int from, int to)
        {
            to = Math.Clamp(to, 0, _actions.Count - 1);
            if (from == to)
                return;
            var item = _actions[from];
            _actions.RemoveAt(from);
            _actions.Insert(to, item);
        }

        private void inputAddEntry(object? sender, EventArgs e)
        {
            var entry = new ActionConfig { ActionName = "", ActionType = ActionConfig.TypeUrl, PathOrUrl = "" };
            int max = _actions.Count + 1;

            using var form = new EditActionForm(entry, max, max, isNew: true);
            if (RunModal(() => form.ShowDialog(this)) != DialogResult.OK)
            {
                RunPendingReload();
                return;
            }

            _actions.Insert(Math.Clamp(form.Position - 1, 0, _actions.Count), form.Result);
            SaveAndRefresh();
        }

        // ------------------------------------------------------------------
        // Drag + drop (reorder cards, drop files/folders from Explorer)
        // ------------------------------------------------------------------

        private void ActionPanel_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Data.GetDataPresent(LinkItem.DragFormat) && e.Data.GetData(LinkItem.DragFormat) is LinkItem dragged)
            {
                _dragging = true;
                e.Effect = DragDropEffects.Move;

                var pt = actionPanel.PointToClient(new Point(e.X, e.Y));
                AutoScrollWhileDragging(pt);

                // Live preview: move the dragged card to the slot under the cursor.
                if (actionPanel.GetChildAtPoint(pt) is LinkItem target && !ReferenceEquals(target, dragged))
                    actionPanel.Controls.SetChildIndex(dragged, actionPanel.Controls.GetChildIndex(target));
                return;
            }

            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void AutoScrollWhileDragging(Point pt)
        {
            const int edge = 30;
            const int step = 20;
            int current = -actionPanel.AutoScrollPosition.Y;

            if (pt.Y < edge)
                actionPanel.AutoScrollPosition = new Point(0, Math.Max(0, current - step));
            else if (pt.Y > actionPanel.ClientSize.Height - edge)
                actionPanel.AutoScrollPosition = new Point(0, current + step);
        }

        private void ActionPanel_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null)
                return;

            if (e.Data.GetDataPresent(LinkItem.DragFormat))
            {
                e.Effect = DragDropEffects.Move; // the order is committed in OnCardDragFinished
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
            {
                var entries = paths.Where(p => File.Exists(p) || Directory.Exists(p))
                                   .Select(ActionConfig.FromPath)
                                   .ToList();
                if (entries.Count == 0)
                    return;

                _actions.AddRange(entries);
                SaveAndRefresh();
            }
        }

        public void OnCardDragFinished(LinkItem card, bool dropped)
        {
            // Read the new order now, apply it after the drag call stack has unwound.
            var newOrder = actionPanel.Controls.OfType<LinkItem>().Select(c => c.Config).ToList();

            BeginInvoke(new Action(() =>
            {
                _dragging = false;

                bool changed = dropped &&
                               newOrder.Count == _actions.Count &&
                               !newOrder.SequenceEqual(_actions);

                if (changed)
                {
                    _actions = newOrder;
                    SaveAndRefresh();
                }
                else
                {
                    PopulateCards(); // cancelled or dropped outside: restore the old order
                    RunPendingReload();
                }
            }));
        }

        // ------------------------------------------------------------------
        // Buttons
        // ------------------------------------------------------------------

        private void inputReloadConfig(object? sender, EventArgs e) => ReloadFromDisk(force: true);

        private void inputLayouts(object? sender, EventArgs e)
        {
            Hide();
            Program.ShowLayoutManager();
        }

        private void inputOpenConfig(object? sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(AppPaths.ActionsFile))
                    _lastKnownText = ActionStore.Save(_actions);

                try
                {
                    Process.Start(new ProcessStartInfo(AppPaths.ActionsFile) { UseShellExecute = true });
                }
                catch
                {
                    // No program associated with .ini
                    Process.Start(new ProcessStartInfo("notepad.exe", $"\"{AppPaths.ActionsFile}\"") { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                ShowError("Error", $"Error opening configuration file:\n{ex.Message}");
            }
        }

        private void inputSearchQuery(object? sender, EventArgs e) => PopulateCards();

        private async void inputUpdate(object? sender, LinkLabelLinkClickedEventArgs e) => await RunUpdateCheckAsync();

        public async Task RunUpdateCheckAsync()
        {
            if (_updateRunning)
                return;

            _updateRunning = true;
            _modalDepth++;
            try
            {
                await Updater.Update.CheckAndMaybeUpdateAsync(this, CancellationToken.None, showNoUpdateMessage: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _modalDepth--;
                _updateRunning = false;
            }
        }

        private void inputShellInteg(object? sender, EventArgs e)
        {
            try
            {
                if (ShellIntegration.IsRegistered)
                {
                    ShellIntegration.Unregister();
                    RunModal(() => MessageBox.Show(this, "\"Add to TrayLink\" was removed from the Explorer context menu.",
                        "Shell Integration", MessageBoxButtons.OK, MessageBoxIcon.Information));
                }
                else
                {
                    ShellIntegration.Register();
                    RunModal(() => MessageBox.Show(this, "You can now right click files or folders and choose \"Add to TrayLink\".\n\n" +
                        "Windows 11: the entry is under \"Show more options\".",
                        "Shell Integration", MessageBoxButtons.OK, MessageBoxIcon.Information));
                }
            }
            catch (Exception ex)
            {
                ShowError("Shell Integration", $"Shell integration failed:\n{ex.Message}");
            }

            UpdateShellButton();
        }

        private void UpdateShellButton() =>
            button1.Text = ShellIntegration.IsRegistered
                ? "Shell Integration: remove \"Add to TrayLink\""
                : "Shell Integration: add \"Add to TrayLink\"";

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _watcher?.Dispose();
                _reloadDebounce.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
