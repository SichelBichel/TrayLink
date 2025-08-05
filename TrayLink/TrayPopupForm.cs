using System.Diagnostics;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using Tomlyn;

namespace TrayLink
{
    public partial class TrayPopupForm : Form
    {

        private List<ActionConfig> actions = new();
        public TrayPopupForm()
        {
            InitializeComponent();
            chkAutoHide.Checked = Properties.Settings.Default.AutoHide;
            PostInit();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Icon = Program.TrayLinkIcon;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
          //  this.Deactivate += (s, e) => this.Hide();
            EventHandler deactivateHandler = (s, e) => this.Hide();

            if (chkAutoHide.Checked)
                this.Deactivate += deactivateHandler;

            chkAutoHide.CheckedChanged += (s, e) =>
            {
                if (chkAutoHide.Checked)
                    this.Deactivate += deactivateHandler;
                else
                    this.Deactivate -= deactivateHandler;

                Properties.Settings.Default.AutoHide = chkAutoHide.Checked;
                Properties.Settings.Default.Save();
            };
            actionPanel.AutoScroll = true;
            actionPanel.HorizontalScroll.Enabled = false;
            actionPanel.HorizontalScroll.Visible = false;
            actionPanel.VerticalScroll.Enabled = true;
            actionPanel.VerticalScroll.Visible = true;
            actionPanel.AutoScrollMinSize = new Size(0, actionPanel.Height + 1);
        }

        private async Task PostInit()
        {
            try
            {
                await Task.Delay(200);
                inputReloadConfig(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during post initialization: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void inputReloadConfig(object sender, EventArgs e)
        {
            string filePath = "actions.ini";
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Configuration file '{filePath}' not found.\nGenerating new Config.", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                createDefaultConfig();
                return;
            }

            try
            {
                actions = LoadActionsFromIni(filePath);
                populateActionCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<ActionConfig> LoadActionsFromIni(string filePath)
        {
            var actions = new List<ActionConfig>();
            ActionConfig current = null;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";"))
                    continue;

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    if (current != null)
                        actions.Add(current);
                    current = new ActionConfig();
                }
                else if (current != null && trimmed.Contains('='))
                {
                    var parts = trimmed.Split('=', 2);
                    var key = parts[0].Trim().ToLower();
                    var value = parts[1].Trim();

                    switch (key)
                    {
                        case "actionname": current.ActionName = value; break;
                        case "actiontype": current.ActionType = value; break;
                        case "pathorurl": current.PathOrUrl = value; break;
                    }
                }
            }

            if (current != null)
                actions.Add(current);

            return actions;
        }


        private void SaveActionsToIni(string filePath, List<ActionConfig> actions)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < actions.Count; i++)
            {
                sb.AppendLine($"[Action{i + 1}]");
                sb.AppendLine($"ActionName={actions[i].ActionName}");
                sb.AppendLine($"ActionType={actions[i].ActionType}");
                sb.AppendLine($"PathOrUrl={actions[i].PathOrUrl}");
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());
            machineReload();
        }
        private void populateActionCards()
        {
            actionPanel.Controls.Clear();
            foreach (var action in actions)
            {
                var card = new LinkItem(action, DeleteAction);
                actionPanel.Controls.Add(card);
            }
        }
        private void DeleteAction(ActionConfig configToDelete)
        {
            var filePath = "actions.ini";
            actions = actions.Where(a => a.ActionName != configToDelete.ActionName).ToList();
            SaveActionsToIni(filePath, actions);
        }
        private async Task machineReload()
        {
            await Task.Delay(250);
            inputReloadConfig(null, null);
        }
        private ActionConfig createDefaultConfig()
        {
            try
            {
                var defaultAction = new ActionConfig
                {
                    ActionName = "Open Notepad",
                    ActionType = "FILE",
                    PathOrUrl = "notepad.exe"
                };

                actions = new List<ActionConfig> { defaultAction };
                SaveActionsToIni("actions.ini", actions);
                return defaultAction;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating default config: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void inputOpenConfig(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("actions.ini") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening configuration file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void inputSearchQuery(object sender, EventArgs e)
        {
            FilteredDeviceCards(searchBar.Text);
        }

        private void FilteredDeviceCards(string searchTerm)
        {
            searchTerm = searchTerm.Trim().ToLower();
            actionPanel.Controls.Clear();

            var filteredActions = actions.Where(s =>
                string.IsNullOrWhiteSpace(searchTerm) ||
                s.ActionName.ToLower().Contains(searchTerm) ||
                s.ActionType.ToLower().Contains(searchTerm) ||
                s.PathOrUrl.ToLower().Contains(searchTerm)).ToList();

            foreach (var action in filteredActions)
            {
                var deviceCard = new LinkItem(action, DeleteAction);
                actionPanel.Controls.Add(deviceCard);
            }
        }

        private void inputUpdate(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://rehoga-interactive.com/traylink/";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening update link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void inputShellInteg(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "Setup.bat",
                    UseShellExecute = true,
                    Verb = "runas"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Shell integration failed:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
