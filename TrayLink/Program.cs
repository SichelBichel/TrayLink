namespace TrayLink
{
    internal static class Program
    {
        static NotifyIcon trayIcon;
        static TrayPopupForm popupForm;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            popupForm = new TrayPopupForm();

            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, 
                Visible = true,
                Text = "TrayLink"
            };

            trayIcon.MouseClick += TrayIcon_MouseClick;

            Application.Run();
        }

        private static void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowPopupForm();
            }
        }

        private static void ShowPopupForm()
        {
            if (popupForm.Visible)
            {
                popupForm.Hide();
                return;
            }

            // positioning
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int x = workingArea.Right - popupForm.Width - 10;
            int y = workingArea.Bottom - popupForm.Height - 10;

            popupForm.Location = new Point(x, y);
            popupForm.Show();
            popupForm.BringToFront();
            popupForm.Activate();
        }
    }
}