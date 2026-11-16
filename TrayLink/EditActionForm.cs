namespace TrayLink
{
    public partial class EditActionForm : Form
    {
        /// <summary>The edited entry (a copy, the original is only changed by the caller).</summary>
        public ActionConfig Result { get; private set; }

        /// <summary>1-based target position in the list.</summary>
        public int Position => (int)numPosition.Value;

        /// <param name="source">Entry to edit, or a new entry.</param>
        /// <param name="position">Current 1-based position.</param>
        /// <param name="maxPosition">Highest valid position (list count, or count + 1 for a new entry).</param>
        /// <param name="isNew">Changes the title only.</param>
        public EditActionForm(ActionConfig source, int position, int maxPosition, bool isNew)
        {
            InitializeComponent();

            Result = source.Clone();
            Text = isNew ? "Add entry" : "Edit entry";
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true; // the tray popup is TopMost, the editor must not open behind it

            textBoxName.Text = source.ActionName;
            textBoxPathOrUrl.Text = source.PathOrUrl;

            comboType.Items.AddRange(ActionConfig.Types);
            var type = source.ActionType.ToUpperInvariant();
            if (!ActionConfig.Types.Contains(type))
                comboType.Items.Add(type); // keep unknown values from a hand edited ini visible
            comboType.SelectedItem = type;

            maxPosition = Math.Max(1, maxPosition);
            numPosition.Minimum = 1;
            numPosition.Maximum = maxPosition;
            numPosition.Value = Math.Clamp(position, 1, maxPosition);
            labelPositionMax.Text = "/ " + maxPosition;

            buttonOk.Click += ButtonOk_Click;
            buttonBrowse.Click += ButtonBrowse_Click;
        }

        private void ButtonOk_Click(object? sender, EventArgs e)
        {
            string name = textBoxName.Text.Trim();
            string path = textBoxPathOrUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show(this, "Name and Path/URL must not be empty.", "Missing input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Result.ActionName = name;
            Result.PathOrUrl = path;
            Result.ActionType = comboType.SelectedItem as string ?? ActionConfig.TypeUrl;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonBrowse_Click(object? sender, EventArgs e)
        {
            var type = comboType.SelectedItem as string;
            var current = Environment.ExpandEnvironmentVariables(textBoxPathOrUrl.Text.Trim().Trim('"'));

            if (type == ActionConfig.TypePath)
            {
                using var dlg = new FolderBrowserDialog { UseDescriptionForTitle = true, Description = "Select folder" };
                if (Directory.Exists(current))
                    dlg.SelectedPath = current;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    ApplyBrowsedPath(dlg.SelectedPath);
            }
            else
            {
                using var dlg = new OpenFileDialog { Title = "Select file", Filter = "All files (*.*)|*.*", CheckFileExists = true };
                try
                {
                    var dir = Path.GetDirectoryName(current);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                        dlg.InitialDirectory = dir;
                }
                catch
                {
                    // invalid characters in the textbox, just open the dialog without a start folder
                }

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ApplyBrowsedPath(dlg.FileName);
                    if (type == ActionConfig.TypeUrl)
                        comboType.SelectedItem = ActionConfig.TypeFile;
                }
            }
        }

        private void ApplyBrowsedPath(string path)
        {
            textBoxPathOrUrl.Text = path;
            if (string.IsNullOrWhiteSpace(textBoxName.Text) || textBoxName.Text == "New Action")
                textBoxName.Text = ActionConfig.FromPath(path).ActionName;
        }
    }
}
