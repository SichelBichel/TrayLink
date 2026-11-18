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

        // Page content (light)
        public static readonly Color Page = Color.FromArgb(236, 239, 244);
        public static readonly Color Card = Color.White;
        public static readonly Color CardLine = Color.FromArgb(206, 213, 226);
        public static readonly Color CardHover = Color.FromArgb(226, 235, 250);
        public static readonly Color DragHighlight = Color.FromArgb(206, 222, 250);
        public static readonly Color AccentText = Color.FromArgb(14, 70, 170);
        public static readonly Color WarningText = Color.FromArgb(180, 100, 0);
        public static readonly Color ErrorText = Color.FromArgb(190, 30, 45);
        public static readonly Color OkText = Color.FromArgb(30, 130, 50);

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
