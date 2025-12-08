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
        public string ActionType { get; private set; } // Hinzugefügt, um den Typ anzuzeigen

        // Konstruktor, der die vorhandene Konfiguration übernimmt
        public EditActionForm(ActionConfig config)
        {
            InitializeComponent();

            textBoxName.Text = config.ActionName;
            textBoxPathOrUrl.Text = config.PathOrUrl;
            //labelActionType.Text = config.ActionType.ToUpper(); // Zum Anzeigen des Typs

            this.ActionType = config.ActionType;
            this.Text = "Eintrag bearbeiten";
            this.StartPosition = FormStartPosition.CenterParent;

            buttonOk.Click += ButtonOk_Click;
        }

        private void ButtonOk_Click(object sender, System.EventArgs e)
        {
            // Speichert die geänderten Werte
            ActionName = textBoxName.Text.Trim();
            PathOrUrl = textBoxPathOrUrl.Text.Trim();

            // Einfache Validierung
            if (string.IsNullOrWhiteSpace(ActionName) || string.IsNullOrWhiteSpace(PathOrUrl))
            {
                MessageBox.Show("Name und Pfad/URL dürfen nicht leer sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

