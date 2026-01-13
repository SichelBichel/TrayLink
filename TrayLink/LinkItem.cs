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
        private Action<ActionConfig> _onEdit;
        private Action<ActionConfig> _onMoveUp;
        private Action<ActionConfig> _onMoveDown;
        public LinkItem(ActionConfig config, Action<ActionConfig> onDelete, Action<ActionConfig> onEdit, Action<ActionConfig> onMoveUp, Action<ActionConfig> onMoveDown)
        {
            InitializeComponent();
            _config = config;
            _onDelete = onDelete;
            _onEdit = onEdit;
            _onMoveUp = onMoveUp;
            _onMoveDown = onMoveDown;
            ApplyConfig();
        }

        private void ApplyConfig()
        {
            labelType.Text = _config.ActionType.ToUpper();
            labelName.Text = _config.ActionName;
            buttonExecute.Click += (s, e) => ExecuteAction(_config);
            buttonEdit.Click += inputEditEntry;

            //buttonUp.Click += ClickMoveUp;
            //buttonDown.Click += clickMoveDown;

           // buttonDelete.Click += inputDeleteEntry;
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

        private void inputEditEntry(object sender, EventArgs e)
        {
            _onEdit?.Invoke(_config);
        }

        private void button2_Click(object sender, EventArgs e){}

        private void ClickMoveUp(object sender, EventArgs e)
        {
            _onMoveUp?.Invoke(_config);
        }

        private void clickMoveDown(object sender, EventArgs e)
        {
            _onMoveDown?.Invoke(_config);
        }
    }
}
