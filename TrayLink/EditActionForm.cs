using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayLink
{
    public partial class EditActionForm : Form
    {
        public string ActionName { get; private set; }
        public string PathOrUrl { get; private set; }
        public string ActionType { get; private set; }

        public EditActionForm(ActionConfig config)
        {
            InitializeComponent();

            textBoxName.Text = config.ActionName;
            textBoxPathOrUrl.Text = config.PathOrUrl;
            this.ActionType = config.ActionType;
            this.Text = "Eintrag bearbeiten";
            this.StartPosition = FormStartPosition.CenterScreen;

            buttonOk.Click += ButtonOk_Click;
        }

        private async void ButtonOk_Click(object sender, System.EventArgs e)
        {
            string newActionName = textBoxName.Text.Trim();
            string newPathOrUrl = textBoxPathOrUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(newActionName) || string.IsNullOrWhiteSpace(newPathOrUrl))
            {
                MessageBox.Show("Name und Pfad/URL dürfen nicht leer sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            ActionName = newActionName;
            PathOrUrl = newPathOrUrl;

            this.DialogResult = DialogResult.OK;

            await Task.Delay(1);

            this.Close();
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}

