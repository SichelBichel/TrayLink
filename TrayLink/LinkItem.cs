using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayLink
{
    public partial class LinkItem : UserControl
    {
        private ActionConfig _config;
        private Action<ActionConfig> _onDelete;
        public LinkItem(ActionConfig config, Action<ActionConfig> onDelete)
        {
            InitializeComponent();
            _config = config;
            _onDelete = onDelete;
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            labelType.Text = _config.ActionType.ToUpper();
            labelName.Text = _config.ActionName;
            buttonExecute.Click += (s, e) => ExecuteAction(_config);
        }

        private void ExecuteAction(ActionConfig config)
        {
            try
            {
                if (config.ActionType.Equals("URL", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo(config.PathOrUrl) { UseShellExecute = true });
                }
                else if (config.ActionType.Equals("FILE", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo(config.PathOrUrl) { UseShellExecute = true });
                }
                else if (config.ActionType.Equals("PATH", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start("explorer.exe", config.PathOrUrl);
                }
                else
                {
                    MessageBox.Show($"Unknown action type: {config.ActionType}", "Action Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing action: {ex.Message}", "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void inputExecute(object sender, EventArgs e)
        {

        }

        private void inputDeleteEntry(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Delete '{_config.ActionName}'?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _onDelete?.Invoke(_config);
            }
        }
    }
}
