using System.Security.AccessControl;
using System.Text.Json;

namespace TrayLink
{
    public partial class TrayPopupForm : Form
    {

        private List<ActionConfig> actions = new();
        public TrayPopupForm()
        {
            InitializeComponent();
            PostInit();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Deactivate += (s, e) => this.Hide();
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
            string filePath = "actions.json";
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Configuration file '{filePath}' not found.\n Generating new Config.", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                createDefaultConfig();
                return;
            }
            try
            {
                string json = File.ReadAllText(filePath);
                var loaded = JsonSerializer.Deserialize<List<ActionConfig>>(json);

                if (loaded != null)
                {
                    actions = loaded;
                    populateActionCards();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void populateActionCards()
        {
            actionPanel.Controls.Clear();
            foreach (var action in actions)
            {
                var card = new LinkItem(action);
                actionPanel.Controls.Add(card);
            }
        }

        private ActionConfig createDefaultConfig()
        {
            try
            {
                var defaultAction = new ActionConfig
                {
                    ActionName = "Default Action",
                    ActionType = "URL/EXE/PATH",
                    PathOrUrl = "Filepath/Path/URL"
                };
                var defaultActions = new List<ActionConfig> { defaultAction };

                string json = JsonSerializer.Serialize(defaultActions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllTextAsync("actions.json", json);

                return defaultAction;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating default config: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }




    /*   private void test(object sender, EventArgs e)
       {

       }*/
}
}
