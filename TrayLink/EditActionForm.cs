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

            // ... (Ihre Konfigurations- und UI-Einstellungen)
            textBoxName.Text = config.ActionName;
            textBoxPathOrUrl.Text = config.PathOrUrl;
            this.ActionType = config.ActionType;
            this.Text = "Eintrag bearbeiten";
            this.StartPosition = FormStartPosition.CenterParent;

            // Nur einmal im Konstruktor anhängen
            buttonOk.Click += ButtonOk_Click;
        }

        // --- Handler für OK-Button ---
        private async void ButtonOk_Click(object sender, System.EventArgs e)
        {
            string newActionName = textBoxName.Text.Trim();
            string newPathOrUrl = textBoxPathOrUrl.Text.Trim();

            // 1. Validierung
            if (string.IsNullOrWhiteSpace(newActionName) || string.IsNullOrWhiteSpace(newPathOrUrl))
            {
                MessageBox.Show("Name und Pfad/URL dürfen nicht leer sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // 2. Werte speichern
            ActionName = newActionName;
            PathOrUrl = newPathOrUrl;

            // 3. DialogResult setzen
            // Das Setzen auf OK signalisiert ShowDialog(), dass die Operation erfolgreich war.
            this.DialogResult = DialogResult.OK;

            // 4. SELBSTZERSTÖRUNG: Asynchroner Force Close
            // Die Verzögerung von 1 Millisekunde (oder sogar 0) gibt dem UI-Thread kurz Zeit, 
            // das DialogResult zu verarbeiten. Danach erzwingen wir das Schließen.
            await Task.Delay(1);

            // Erzwingt das sofortige Schließen des Formulars.
            this.Close();
        }


        // --- Handler für das Kreuz (X) ---
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Wenn das Formular ohne DialogResult geschlossen wird (z.B. durch X), 
            // setzen wir es explizit auf Cancel, um sicherzustellen, dass ShowDialog() zurückkehrt.
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}

