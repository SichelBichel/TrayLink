namespace TrayLink
{
    public partial class TrayPopupForm : Form
    {
        public TrayPopupForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Deactivate += (s, e) => this.Hide();
        }

        private void test(object sender, EventArgs e)
        {

        }
    }
}
