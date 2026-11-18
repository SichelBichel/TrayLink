using TrayLink.Core.Layouts;

namespace TrayLink
{
    /// <summary>What a layout card needs from the Desktops tab.</summary>
    public interface ILayoutCardHost
    {
        void RestoreLayout(DesktopLayout layout);
        void UpdateLayout(DesktopLayout layout);
        void EditLayout(DesktopLayout layout);
    }

    /// <summary>One saved desktop layout in the Desktops tab of the popup.</summary>
    public partial class LayoutCard : UserControl
    {
        private readonly ILayoutCardHost _host;

        public DesktopLayout Snapshot { get; }

        public LayoutCard(DesktopLayout layout, bool restoredAtStart, ILayoutCardHost host)
        {
            InitializeComponent();
            Theme.ApplyContent(this);
            Snapshot = layout;
            _host = host;

            labelName.Text = layout.Name;
            int windows = layout.Windows.Count(w => w.Include);
            labelInfo.Text = $"{windows} window{(windows == 1 ? "" : "s")}, {layout.Monitors.Count} monitor{(layout.Monitors.Count == 1 ? "" : "s")}";

            // Second line: the start flag, otherwise when it was saved.
            labelAuto.Visible = true;
            if (!restoredAtStart)
            {
                labelAuto.Text = "Saved " + layout.SavedAt.ToString("dd.MM.yyyy HH:mm");
                labelAuto.Font = labelInfo.Font;
                labelAuto.ForeColor = Theme.MutedText;
            }

            var apps = layout.Windows.Where(w => w.Include).Select(w => w.AppName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var tip = layout.Name + "\n" + string.Join(", ", apps.Take(12)) + (apps.Count > 12 ? " ..." : "");
            toolTip.SetToolTip(this, tip);
            toolTip.SetToolTip(labelName, tip);
            toolTip.SetToolTip(labelInfo, tip);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Theme.PaintCardFrame(e.Graphics, ClientRectangle, highlight: false);
            Theme.PaintMarker(e.Graphics, new Rectangle(0, 0, 3, Height));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        private void buttonRestore_Click(object? sender, EventArgs e) => _host.RestoreLayout(Snapshot);

        private void buttonUpdate_Click(object? sender, EventArgs e) => _host.UpdateLayout(Snapshot);

        private void buttonEdit_Click(object? sender, EventArgs e) => _host.EditLayout(Snapshot);
    }
}
