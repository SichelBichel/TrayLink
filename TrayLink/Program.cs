using System.Reflection;

namespace TrayLink
{
    internal static class Program
    {
        public static Icon TrayLinkIcon { get; private set; }
        static NotifyIcon trayIcon;
        static TrayPopupForm popupForm;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            popupForm = new TrayPopupForm()
            {
                Icon = TrayLinkIcon
            };

            trayIcon = new NotifyIcon
            {
                Icon = LoadIconFromResources("TrayLink.Resources.TrayLink.ico"),
                Visible = true,
                Text = "TrayLink"
            };

            trayIcon.MouseClick += TrayIcon_MouseClick;

            Application.Run();
        }

        private static Icon LoadIconFromResources(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                    throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

                return new Icon(stream);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tray icon:\n{ex.Message}", "Icon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return SystemIcons.Application; // fallback icon
            }
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
            if (popupForm == null || popupForm.IsDisposed)
            {
                popupForm = new TrayPopupForm
                {
                    Icon = LoadIconFromResources("TrayLink.Resources.TrayLink.ico")
                };
            }

            if (popupForm.Visible)
            {
                popupForm.Hide();
                return;
            }

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