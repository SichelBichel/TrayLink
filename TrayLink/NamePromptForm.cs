namespace TrayLink
{
    /// <summary>Asks for a name (new layout, rename).</summary>
    public partial class NamePromptForm : Form
    {
        public string Value => textName.Text.Trim();

        public NamePromptForm(string title, string prompt, string initial)
        {
            InitializeComponent();
            Text = title;
            labelPrompt.Text = prompt;
            textName.Text = initial;
            Icon = Program.AppIcon;
            TopMost = true;

            buttonOk.Click += (s, e) =>
            {
                if (Value.Length == 0)
                {
                    MessageBox.Show(this, "Please enter a name.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };

            Shown += (s, e) =>
            {
                textName.Focus();
                textName.SelectAll();
            };
        }

        /// <summary>Shows the prompt, returns null when cancelled.</summary>
        public static string? Ask(IWin32Window? owner, string title, string prompt, string initial)
        {
            using var form = new NamePromptForm(title, prompt, initial);
            return form.ShowDialog(owner) == DialogResult.OK ? form.Value : null;
        }
    }
}
