using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace TrayLink
{
    /// <summary>
    /// TrayLink colours: the ReHoGa chrome of AutoBak Server and DeskInfo (almost black navy for header, tabs, status bar,
    /// menus) with a dark blue accent. Violet / pink is AutoBak, yellow / orange is DeskInfo. Same structure as the
    /// AutoBak Server GUI Theme, so the copied ChromeRenderer works. Page content is light. Square corners everywhere.
    /// The designer files use the same values as literal colours, so Visual Studio shows the real look.
    /// </summary>
    internal static class Theme
    {
        // Chrome (navy)
        public static readonly Color Chrome = Color.FromArgb(12, 16, 28);
        public static readonly Color ChromeRaised = Color.FromArgb(20, 28, 48);
        public static readonly Color Hover = Color.FromArgb(32, 46, 78);
        public static readonly Color Selected = Color.FromArgb(26, 50, 104);
        public static readonly Color Line = Color.FromArgb(52, 66, 100);
        public static readonly Color Text = Color.FromArgb(238, 242, 250);
        public static readonly Color Muted = Color.FromArgb(158, 172, 198);
        public static readonly Color Disabled = Color.FromArgb(100, 110, 132);

        // Accent (dark blue)
        public static readonly Color Blue = Color.FromArgb(0, 52, 160);
        public static readonly Color BlueBright = Color.FromArgb(24, 118, 232);
        public static readonly Color BlueLight = Color.FromArgb(120, 172, 255);   // ticks, frames and icons on the chrome

        /// <summary>
        /// Dark page content. Set once at the start by <see cref="ColorModeSupport"/> (Options > Appearance, then a restart):
        /// Windows Forms switches its own controls with Application.SetColorMode, the colours below follow.
        /// </summary>
        public static bool IsDark { get; set; }

        // Page content, light values (also the literal colours in the designer files)
        public static readonly Color PageLight = Color.FromArgb(236, 239, 244);
        public static readonly Color CardLight = Color.FromArgb(255, 255, 255);
        public static readonly Color CardLineLight = Color.FromArgb(206, 213, 226);
        public static readonly Color CardHoverLight = Color.FromArgb(226, 235, 250);
        public static readonly Color ActionBackLight = Color.FromArgb(226, 235, 250);   // big open button of a link card
        public static readonly Color ActionHoverLight = Color.FromArgb(206, 222, 250);
        public static readonly Color ContentTextLight = Color.FromArgb(20, 28, 48);
        public static readonly Color AccentTextLight = Color.FromArgb(14, 70, 170);
        public static readonly Color GridLineLight = Color.FromArgb(220, 226, 236);

        // Launch button of a link card: white in light AND dark mode (owner decision, 1.7.1)
        public static readonly Color LaunchBack = Color.White;
        public static readonly Color LaunchHover = Color.FromArgb(226, 235, 250);

        // Page content, dark values
        private static readonly Color PageDark = Color.FromArgb(24, 28, 38);
        private static readonly Color CardDark = Color.FromArgb(34, 40, 54);
        private static readonly Color CardLineDark = Color.FromArgb(60, 70, 94);
        private static readonly Color CardHoverDark = Color.FromArgb(46, 58, 86);
        private static readonly Color ActionBackDark = Color.FromArgb(92, 120, 178);
        private static readonly Color ActionHoverDark = Color.FromArgb(114, 142, 204);
        private static readonly Color ContentTextDark = Color.FromArgb(228, 234, 244);
        private static readonly Color AccentTextDark = Color.FromArgb(128, 178, 255);
        private static readonly Color MutedTextDark = Color.FromArgb(150, 162, 186);
        private static readonly Color InputBackDark = Color.FromArgb(18, 22, 32);
        private static readonly Color GridLineDark = Color.FromArgb(54, 62, 84);

        public static Color Page => IsDark ? PageDark : PageLight;
        public static Color Card => IsDark ? CardDark : CardLight;
        public static Color CardLine => IsDark ? CardLineDark : CardLineLight;
        public static Color CardHover => IsDark ? CardHoverDark : CardHoverLight;
        public static Color DragHighlight => IsDark ? Color.FromArgb(40, 64, 112) : Color.FromArgb(206, 222, 250);
        public static Color ContentText => IsDark ? ContentTextDark : ContentTextLight;
        public static Color AccentText => IsDark ? AccentTextDark : AccentTextLight;
        public static Color MutedText => IsDark ? MutedTextDark : SystemColors.GrayText;
        public static Color GridLine => IsDark ? GridLineDark : GridLineLight;
        public static Color WarningText => IsDark ? Color.FromArgb(255, 176, 64) : Color.FromArgb(180, 100, 0);
        public static Color ErrorText => IsDark ? Color.FromArgb(255, 110, 120) : Color.FromArgb(190, 30, 45);
        public static Color OkText => IsDark ? Color.FromArgb(110, 210, 130) : Color.FromArgb(30, 130, 50);

        /// <summary>The dark blue to blue line under the header band and under a selected tab.</summary>
        public static void PaintAccent(Graphics g, Rectangle r)
        {
            if (r.Width <= 0 || r.Height <= 0) return;
            using var brush = new LinearGradientBrush(r, Blue, BlueBright, LinearGradientMode.Horizontal);
            g.FillRectangle(brush, r);
        }

        /// <summary>Left edge marker of a selected navigation item (used by the copied ChromeRenderer).</summary>
        public static void PaintMarker(Graphics g, Rectangle r)
        {
            if (r.Width <= 0 || r.Height <= 0) return;
            using var brush = new LinearGradientBrush(r, Blue, BlueBright, LinearGradientMode.Vertical);
            g.FillRectangle(brush, r);
        }

        /// <summary>Sets the chrome renderer on tool strips (a renderer cannot be set in the designer).</summary>
        public static void ApplyRenderer(params ToolStrip[] strips)
        {
            var renderer = new Controls.ChromeRenderer();
            foreach (var strip in strips)
            {
                strip.Renderer = renderer;
                if (strip is not ToolStripDropDown)
                {
                    strip.BackColor = Chrome;
                    strip.ForeColor = Text;
                }
            }
        }

        /// <summary>
        /// Dark mode for the page content of a form or card. The designer files hold the light colours; this maps them
        /// to their dark values (the chrome is dark in both modes and stays as it is). Call it at the end of the
        /// constructor, after InitializeComponent. Does nothing in light mode.
        /// </summary>
        public static void ApplyContent(Control root)
        {
            if (!IsDark)
                return;
            Walk(root);
        }

        private static void Walk(Control c)
        {
            if (c is ToolStrip)
                return; // the chrome renderer draws these
            if (c.Tag is "keep")
                return; // fixed colours on purpose (launch button of a link card)

            switch (c)
            {
                case TextBoxBase or ListBox or ComboBox or NumericUpDown:
                    c.BackColor = InputBackDark;
                    c.ForeColor = ContentTextDark;
                    break;

                case DataGridView grid:
                    grid.BackgroundColor = CardDark;
                    grid.GridColor = GridLineDark;
                    grid.DefaultCellStyle.BackColor = CardDark;
                    grid.DefaultCellStyle.ForeColor = ContentTextDark;
                    grid.ForeColor = ContentTextDark;
                    break;

                default:
                    var back = MapBack(c.BackColor);
                    if (back.ToArgb() != c.BackColor.ToArgb())
                        c.BackColor = back;
                    var fore = MapFore(c.ForeColor);
                    if (fore != c.ForeColor)
                        c.ForeColor = fore;
                    break;
            }

            if (c is ButtonBase { FlatStyle: FlatStyle.Flat } b)
            {
                b.FlatAppearance.BorderColor = MapBack(b.FlatAppearance.BorderColor);
                b.FlatAppearance.MouseOverBackColor = MapBack(b.FlatAppearance.MouseOverBackColor);
            }

            foreach (Control child in c.Controls)
                Walk(child);
        }

        private static Color MapBack(Color c)
        {
            if (c.IsEmpty) return c;
            int argb = c.ToArgb();
            if (argb == PageLight.ToArgb()) return PageDark;
            if (argb == CardLight.ToArgb()) return CardDark;
            if (argb == CardLineLight.ToArgb()) return CardLineDark;
            if (argb == ActionBackLight.ToArgb()) return ActionBackDark;      // same value as CardHoverLight
            if (argb == ActionHoverLight.ToArgb()) return ActionHoverDark;
            return c;
        }

        private static Color MapFore(Color c)
        {
            if (c == SystemColors.GrayText) return MutedTextDark;
            if (c == SystemColors.ControlText || c == SystemColors.WindowText) return ContentTextDark;
            int argb = c.ToArgb();
            if (argb == ContentTextLight.ToArgb()) return ContentTextDark;
            if (argb == AccentTextLight.ToArgb()) return AccentTextDark;
            return c;
        }

        /// <summary>The launch button keeps its white tile in every mode (call after <see cref="ApplyContent"/>).</summary>
        public static void KeepLaunchWhite(ButtonBase button)
        {
            button.BackColor = LaunchBack;
            button.FlatAppearance.MouseOverBackColor = LaunchHover;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(206, 222, 250);
        }

        /// <summary>Thin frame around a card (white control on the light page).</summary>
        public static void PaintCardFrame(Graphics g, Rectangle client, bool highlight)
        {
            using var pen = new Pen(highlight ? BlueBright : CardLine);
            g.DrawRectangle(pen, 0, 0, client.Width - 1, client.Height - 1);
        }

        /// <summary>A copy of a (black) logo in one colour, keeping its transparency. For logos on the navy chrome.</summary>
        public static Bitmap Tint(Image source, Color color, int height)
        {
            int width = Math.Max(1, source.Width * height / Math.Max(1, source.Height));
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bmp);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            using var attributes = new ImageAttributes();
            attributes.SetColorMatrix(new ColorMatrix(new[]
            {
                new float[] { 0, 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { color.R / 255f, color.G / 255f, color.B / 255f, 0, 1 }
            }));
            g.DrawImage(source, new Rectangle(0, 0, width, height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            return bmp;
        }
    }
}
