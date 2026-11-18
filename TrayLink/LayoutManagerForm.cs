using TrayLink.Core;
using TrayLink.Core.Layouts;

namespace TrayLink
{
    /// <summary>List of the saved desktop layouts and the windows of the selected one.</summary>
    public partial class LayoutManagerForm : Form
    {
        private const string StartAuto = "Auto";
        private const string StartPositionOnly = "Position only";

        private List<DesktopLayout> _layouts = new();
        private bool _filling;

        private DesktopLayout? Current => listLayouts.SelectedItem as DesktopLayout;

        private readonly string? _selectOnLoad;

        public LayoutManagerForm(string? select = null)
        {
            InitializeComponent();
            _selectOnLoad = select;
            grid.DefaultCellStyle.SelectionBackColor = Theme.Selected;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            Theme.ApplyContent(this);
            Icon = Program.AppIcon;
            listLayouts.DisplayMember = nameof(DesktopLayout.Name);

            listLayouts.SelectedIndexChanged += (s, e) => ShowLayout();
            listLayouts.DoubleClick += async (s, e) => await RestoreCurrentAsync();
            buttonSaveNew.Click += async (s, e) => await SaveNewAsync();
            buttonRename.Click += (s, e) => RenameCurrent();
            buttonDelete.Click += (s, e) => DeleteCurrent();
            buttonUpdate.Click += async (s, e) => await UpdateCurrentAsync();
            buttonRestore.Click += async (s, e) => await RestoreCurrentAsync();
            buttonClose.Click += (s, e) => Close();
            chkAutoRestore.CheckedChanged += ChkAutoRestore_CheckedChanged;

            grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (grid.IsCurrentCellDirty)
                    grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            grid.CellValueChanged += Grid_CellValueChanged;
            grid.DataError += (s, e) => e.ThrowException = false;

            LayoutUi.LayoutsChanged += OnLayoutsChanged;
            FormClosed += (s, e) => LayoutUi.LayoutsChanged -= OnLayoutsChanged;
            Load += (s, e) => ReloadList(_selectOnLoad);
        }

        public void SelectLayout(string name) => ReloadList(name);

        private void pnlAccent_Paint(object? sender, PaintEventArgs e) => Theme.PaintAccent(e.Graphics, pnlAccent.ClientRectangle);

        private void pnlAccent_Resize(object? sender, EventArgs e) => pnlAccent.Invalidate();

        private void OnLayoutsChanged()
        {
            if (IsHandleCreated && !IsDisposed)
                BeginInvoke(new Action(() => ReloadList(Current?.Name)));
        }

        private void ReloadList(string? select)
        {
            _layouts = LayoutStore.LoadAll();
            listLayouts.BeginUpdate();
            listLayouts.Items.Clear();
            foreach (var l in _layouts)
                listLayouts.Items.Add(l);
            listLayouts.EndUpdate();

            int index = select == null ? -1 : _layouts.FindIndex(l => l.Name.Equals(select, StringComparison.CurrentCultureIgnoreCase));
            if (index < 0 && _layouts.Count > 0)
                index = 0;
            listLayouts.SelectedIndex = index;
            ShowLayout();
        }

        private void ShowLayout()
        {
            _filling = true;
            try
            {
                grid.Rows.Clear();
                var layout = Current;
                bool has = layout != null;
                buttonRename.Enabled = buttonDelete.Enabled = buttonUpdate.Enabled = buttonRestore.Enabled = chkAutoRestore.Enabled = has;

                if (layout == null)
                {
                    labelInfo.Text = _layouts.Count == 0
                        ? "No layout yet. Arrange your windows, then \"Save current desktop...\"."
                        : "No layout selected";
                    chkAutoRestore.Checked = false;
                    return;
                }

                labelInfo.Text = $"Saved {layout.SavedAt:g}   |   {layout.Monitors.Count} monitor(s)   |   " +
                                 $"{layout.Windows.Count} window(s), {layout.Windows.Count(w => w.CanStart)} can be started";
                chkAutoRestore.Checked = string.Equals(AppSettings.Current.AutoRestoreLayout, layout.Name, StringComparison.CurrentCultureIgnoreCase);

                foreach (var w in layout.Windows)
                {
                    var monitor = layout.Monitors.FirstOrDefault(m => m.Index == w.MonitorIndex);
                    int i = grid.Rows.Add(
                        w.Include,
                        w.AppName,
                        w.Title,
                        monitor?.ToString() ?? (w.MonitorIndex + 1).ToString(),
                        w.State.ToString(),
                        w.CanStart && w.Start == StartMode.Auto ? StartAuto : StartPositionOnly,
                        w.StartWith,
                        w.DelaySeconds.ToString());

                    var row = grid.Rows[i];
                    row.Tag = w;
                    row.Cells[colApp.Index].ToolTipText = w.ExePath;
                    row.Cells[colTitle.Index].ToolTipText = w.Title;
                    row.Cells[colStartWith.Index].ToolTipText = StartWithHint(w);

                    if (!w.CanStart)
                    {
                        row.DefaultCellStyle.ForeColor = Theme.MutedText;
                        row.Cells[colStart.Index].ReadOnly = true;
                        row.Cells[colDelay.Index].ReadOnly = true;
                    }
                    if (w.Launch is LaunchKind.None or LaunchKind.PackagedApp or LaunchKind.Squirrel)
                        row.Cells[colStartWith.Index].ReadOnly = true;
                }
            }
            finally
            {
                _filling = false;
            }
        }

        private static string StartWithHint(LayoutWindow w) => w.Launch switch
        {
            LaunchKind.Exe => "Arguments for " + Path.GetFileName(w.ExePath) + " (for example the project or file to open)",
            LaunchKind.Explorer => "Folder of the Explorer window" + (w.ExtraPaths.Count > 0 ? "\nOther tabs: " + string.Join(", ", w.ExtraPaths) : ""),
            LaunchKind.PackagedApp => "Store app, started by its app id",
            LaunchKind.Squirrel => "Started through Update.exe (the folder of the app changes with every update)",
            _ => "Can not be started (system window or no access): only placed when it is open"
        };

        private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_filling || e.RowIndex < 0 || Current == null)
                return;

            var row = grid.Rows[e.RowIndex];
            if (row.Tag is not LayoutWindow w)
                return;

            var value = row.Cells[e.ColumnIndex].Value;
            if (e.ColumnIndex == colInclude.Index)
            {
                w.Include = value is true;
            }
            else if (e.ColumnIndex == colStart.Index)
            {
                w.Start = value as string == StartAuto && w.CanStart ? StartMode.Auto : StartMode.PositionOnly;
            }
            else if (e.ColumnIndex == colStartWith.Index)
            {
                w.StartWith = (value as string ?? "").Trim();
            }
            else if (e.ColumnIndex == colDelay.Index)
            {
                if (int.TryParse(value as string, out int delay) && delay >= 0 && delay <= 600)
                    w.DelaySeconds = delay;
                else
                {
                    _filling = true;
                    row.Cells[e.ColumnIndex].Value = w.DelaySeconds.ToString();
                    _filling = false;
                    return;
                }
            }
            else
            {
                return;
            }

            SaveCurrent();
        }

        private void SaveCurrent()
        {
            if (Current == null)
                return;
            try
            {
                LayoutStore.Save(Current);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not save the layout:\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveNewAsync()
        {
            SetBusy(true);
            try
            {
                var layout = await LayoutUi.SaveCurrentAsync(this);
                if (layout != null)
                    ReloadList(layout.Name);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task UpdateCurrentAsync()
        {
            var layout = Current;
            if (layout == null)
                return;
            if (MessageBox.Show(this, $"Take the windows that are open now for '{layout.Name}'?\n\n" +
                    "Use, Start, Delay and own \"Start with\" values of known windows are kept.",
                    Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            SetBusy(true);
            try
            {
                var updated = await LayoutUi.UpdateAsync(layout, this);
                if (updated != null)
                    ReloadList(updated.Name);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task RestoreCurrentAsync()
        {
            var layout = Current;
            if (layout == null || LayoutUi.Busy)
                return;

            SetBusy(true);
            try
            {
                await LayoutUi.RestoreAsync(layout, atLogin: false);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void RenameCurrent()
        {
            var layout = Current;
            if (layout == null)
                return;

            var name = NamePromptForm.Ask(this, "Rename layout", "New name:", layout.Name);
            if (name == null || name == layout.Name)
                return;
            if (LayoutStore.Exists(name) && !name.Equals(layout.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show(this, $"A layout '{name}' exists already.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool wasAuto = string.Equals(AppSettings.Current.AutoRestoreLayout, layout.Name, StringComparison.CurrentCultureIgnoreCase);
            try
            {
                LayoutStore.Rename(layout, name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (wasAuto)
            {
                AppSettings.Current.AutoRestoreLayout = name;
                AppSettings.Save();
            }
            LayoutUi.NotifyChanged();
            ReloadList(name);
        }

        private void DeleteCurrent()
        {
            var layout = Current;
            if (layout == null)
                return;
            if (MessageBox.Show(this, $"Delete the layout '{layout.Name}'?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                LayoutStore.Delete(layout.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.Equals(AppSettings.Current.AutoRestoreLayout, layout.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                AppSettings.Current.AutoRestoreLayout = null;
                AppSettings.Save();
            }
            LayoutUi.NotifyChanged();
            ReloadList(null);
        }

        private void ChkAutoRestore_CheckedChanged(object? sender, EventArgs e)
        {
            if (_filling || Current == null)
                return;

            if (chkAutoRestore.Checked)
            {
                AppSettings.Current.AutoRestoreLayout = Current.Name;
                if (!Autostart.IsEnabled &&
                    MessageBox.Show(this, "TrayLink does not start with Windows yet, so the layout would not be restored.\n\nTurn on \"Start with Windows\" now?",
                        Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        Autostart.SetEnabled(true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (string.Equals(AppSettings.Current.AutoRestoreLayout, Current.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                AppSettings.Current.AutoRestoreLayout = null;
            }
            AppSettings.Save();
        }

        private void SetBusy(bool busy)
        {
            UseWaitCursor = busy;
            foreach (var b in new Control[] { buttonSaveNew, buttonRename, buttonDelete, buttonUpdate, buttonRestore, listLayouts })
                b.Enabled = !busy && (b == buttonSaveNew || b == listLayouts || Current != null);
        }
    }
}
