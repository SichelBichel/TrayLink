namespace TrayLink.Controls;

/// <summary>
/// TrayLink: copy of the AutoBak Server GUI file (src\AutoBak.Gui\Controls\ChromeRenderer.cs). Changed: namespace,
/// Theme.VioletLight is Theme.BlueLight, and the tab strip of the popup (a ToolStrip with Tag "tabs": checked button =
/// raised fill, the accent line of the popup lies directly below). Copy it again when it changes there and keep these differences.
/// Original summary:
/// Renders the AutoBak chrome (menu with its drop downs, sidebar navigation, header tool strip, status bar, context menus)
/// in the navy of the logo (phase 9a). Square corners everywhere. The selected navigation item gets a raised fill and
/// the violet to magenta marker on its left edge; section captions in the sidebar are muted. A button with a Color in
/// its Tag is filled with that colour (scheduler on/off, 9d); other buttons of the header tool strip are drawn as raised
/// square buttons with a frame.
/// Set by MainForm on its tool strips (<see cref="Theme.ApplyRenderer"/>; a renderer cannot be set in the designer).
/// Modelled on the Command Center's ChromeRenderer, without its rounded highlights.
/// </summary>
internal sealed class ChromeRenderer : ToolStripProfessionalRenderer
{
    public ChromeRenderer()
        : base(new ChromeColors())
    {
        RoundedEdges = false;
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        if (e.ToolStrip is ToolStripDropDown)
        {
            base.OnRenderToolStripBackground(e);
            return;
        }
        using var brush = new SolidBrush(e.BackColor);
        e.Graphics.FillRectangle(brush, e.AffectedBounds);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        if (e.ToolStrip is not ToolStripDropDown) return;
        // Square one pixel frame for menus and context menus.
        using var pen = new Pen(Theme.Line);
        var r = e.AffectedBounds;
        e.Graphics.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item is not ToolStripButton button) return;
        var r = new Rectangle(Point.Empty, e.Item.Size);
        var nav = IsNavigation(e.ToolStrip);

        // TrayLink: tabs of the popup (Links / Desktops).
        if (IsTabStrip(e.ToolStrip))
        {
            if (button.Checked)
                Fill(e.Graphics, r, Theme.Selected); // the accent line follows directly below the strip
            else if (button.Selected || button.Pressed)
            {
                Fill(e.Graphics, r, Theme.Hover);
            }
            return;
        }

        // A state button (9d: scheduler on/off) carries its colour in Tag: solid fill, lighter on hover.
        if (button.Tag is Color state && button.Enabled)
        {
            Fill(e.Graphics, r, button.Pressed ? ControlPaint.Dark(state, 0.1f) : button.Selected ? ControlPaint.Light(state, 0.25f) : state);
            return;
        }

        // Header tool strip (9d follow-up): real buttons, a raised square with a one pixel frame.
        if (IsButtonBar(e.ToolStrip))
        {
            var fill = !button.Enabled ? Theme.ChromeRaised
                : button.Pressed ? Theme.Selected
                : button.Selected ? Theme.Selected
                : Theme.Hover;
            Fill(e.Graphics, r, fill);
            using var frame = new Pen(button.Selected && button.Enabled ? Theme.BlueLight : Theme.Line);
            e.Graphics.DrawRectangle(frame, 0, 0, r.Width - 1, r.Height - 1);
            return;
        }

        if (button.Checked)
        {
            Fill(e.Graphics, r, Theme.Selected);
            if (nav) Theme.PaintMarker(e.Graphics, new Rectangle(0, r.Y + 1, 4, r.Height - 2));
        }
        else if ((button.Selected || button.Pressed) && button.Enabled)
        {
            Fill(e.Graphics, r, button.Pressed ? Theme.Selected : Theme.Hover);
        }
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        var r = new Rectangle(Point.Empty, e.Item.Size);
        if (e.ToolStrip is ToolStripDropDown)
        {
            if (e.Item.Selected && e.Item.Enabled) Fill(e.Graphics, new Rectangle(2, 0, r.Width - 4, r.Height), Theme.Hover);
            return;
        }
        // Top level menu items.
        if (e.Item is ToolStripMenuItem { DropDown.Visible: true }) Fill(e.Graphics, r, Theme.Selected);
        else if (e.Item.Selected) Fill(e.Graphics, r, Theme.Hover);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (e.Item is ToolStripLabel && IsNavigation(e.ToolStrip))
            e.TextColor = Theme.Muted; // section captions
        else if (e.Item is ToolStripButton { Checked: true })
            e.TextColor = Color.White;
        else if (e.Item is ToolStripStatusLabel)
            e.TextColor = e.Item.ForeColor == SystemColors.ControlText ? Theme.Text : e.Item.ForeColor; // status colours stay
        else
            e.TextColor = e.Item.Enabled ? Theme.Text : Theme.Disabled;
        base.OnRenderItemText(e);
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        e.ArrowColor = e.Item?.Enabled == false ? Theme.Disabled : Theme.Muted;
        base.OnRenderArrow(e);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        // Checked menu items (View > page): square violet box with a light tick.
        var r = e.ImageRectangle;
        Fill(e.Graphics, r, Theme.Selected);
        using var pen = new Pen(Theme.BlueLight, 2f);
        var g = e.Graphics;
        var old = g.SmoothingMode;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.DrawLines(pen, [
            new Point(r.X + r.Width / 4, r.Y + r.Height / 2),
            new Point(r.X + r.Width * 5 / 12, r.Bottom - r.Height / 4),
            new Point(r.Right - r.Width / 4, r.Y + r.Height / 4)]);
        g.SmoothingMode = old;
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        var r = new Rectangle(Point.Empty, e.Item.Size);
        using var pen = new Pen(Theme.Line);
        if (e.Vertical)
            e.Graphics.DrawLine(pen, r.Width / 2, 6, r.Width / 2, r.Height - 6);
        else
            e.Graphics.DrawLine(pen, 4, r.Height / 2, r.Width - 4, r.Height / 2);
    }

    protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
    {
        // no grip dots on the navy status bar
    }

    /// <summary>A horizontal tool strip that is no menu, status bar or drop down: its buttons are drawn as buttons.</summary>
    private static bool IsButtonBar(ToolStrip strip) =>
        strip is not (MenuStrip or StatusStrip or ToolStripDropDown) && strip.LayoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow &&
        !IsTabStrip(strip);

    /// <summary>TrayLink: the tab strip of the popup carries the Tag "tabs".</summary>
    private static bool IsTabStrip(ToolStrip strip) => strip.Tag is "tabs";

    /// <summary>The vertical navigation strip: wide items with a marker.</summary>
    private static bool IsNavigation(ToolStrip strip) => strip.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow;

    private static void Fill(Graphics g, Rectangle r, Color color)
    {
        using var brush = new SolidBrush(color);
        g.FillRectangle(brush, r);
    }

    /// <summary>Colours of menus and drop downs.</summary>
    private sealed class ChromeColors : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Theme.ChromeRaised;
        public override Color ImageMarginGradientBegin => Theme.ChromeRaised;
        public override Color ImageMarginGradientMiddle => Theme.ChromeRaised;
        public override Color ImageMarginGradientEnd => Theme.ChromeRaised;
        public override Color MenuBorder => Theme.Line;
        public override Color MenuItemBorder => Theme.Selected;
        public override Color MenuItemSelected => Theme.Hover;
        public override Color MenuItemSelectedGradientBegin => Theme.Hover;
        public override Color MenuItemSelectedGradientEnd => Theme.Hover;
        public override Color MenuItemPressedGradientBegin => Theme.Selected;
        public override Color MenuItemPressedGradientMiddle => Theme.Selected;
        public override Color MenuItemPressedGradientEnd => Theme.Selected;
        public override Color MenuStripGradientBegin => Theme.Chrome;
        public override Color MenuStripGradientEnd => Theme.Chrome;
        public override Color SeparatorDark => Theme.Line;
        public override Color SeparatorLight => Theme.Line;
        public override Color CheckBackground => Theme.Selected;
        public override Color CheckSelectedBackground => Theme.Selected;
        public override Color CheckPressedBackground => Theme.Selected;
        public override Color ButtonSelectedHighlight => Theme.Hover;
        public override Color ButtonSelectedBorder => Theme.Hover;
        public override Color ButtonCheckedHighlight => Theme.Selected;
        public override Color StatusStripGradientBegin => Theme.Chrome;
        public override Color StatusStripGradientEnd => Theme.Chrome;
    }
}
