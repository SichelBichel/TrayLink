namespace TrayLink.Core.Layouts
{
    /// <summary>
    /// Maps the monitors of a saved layout to the monitors that are connected now.
    /// Same device name and same size = the same monitor. A monitor that is missing or changed gets the
    /// best match and the window is scaled into its work area.
    /// </summary>
    internal sealed class MonitorMapper
    {
        private readonly Dictionary<int, MonitorSnapshot> _map = new();
        private readonly List<MonitorSnapshot> _saved;
        private readonly List<MonitorSnapshot> _current;

        /// <summary>True when every saved monitor exists with the same position and size.</summary>
        public bool Identical { get; }

        public MonitorMapper(List<MonitorSnapshot> saved, List<MonitorSnapshot> current)
        {
            _saved = saved;
            _current = current;

            bool identical = saved.Count > 0 && current.Count > 0;
            var used = new HashSet<int>();

            foreach (var s in saved)
            {
                var sb = s.Bounds.ToRectangle();
                var target =
                    current.FirstOrDefault(c => !used.Contains(c.Index) && c.DeviceName == s.DeviceName && c.Bounds.ToRectangle() == sb) ??
                    current.FirstOrDefault(c => !used.Contains(c.Index) && c.Bounds.ToRectangle() == sb) ??
                    current.FirstOrDefault(c => !used.Contains(c.Index) && c.DeviceName == s.DeviceName) ??
                    current.FirstOrDefault(c => !used.Contains(c.Index) && c.Index == s.Index) ??
                    current.FirstOrDefault(c => c.Primary) ??
                    current.FirstOrDefault();

                if (target == null)
                {
                    identical = false;
                    continue;
                }

                used.Add(target.Index);
                _map[s.Index] = target;
                if (target.Bounds.ToRectangle() != sb || target.WorkArea.ToRectangle() != s.WorkArea.ToRectangle())
                    identical = false;
            }

            Identical = identical;
        }

        /// <summary>Target normal rectangle of a saved window on the current monitors.</summary>
        public Rectangle MapBounds(LayoutWindow w)
        {
            var bounds = w.Bounds.ToRectangle();
            var source = _saved.FirstOrDefault(m => m.Index == w.MonitorIndex);

            if (source == null || !_map.TryGetValue(source.Index, out var target))
            {
                // Layout without monitor info: keep the rectangle, but make sure it is visible somewhere.
                return KeepVisible(bounds, _current.FirstOrDefault(c => c.Primary) ?? _current.FirstOrDefault());
            }

            var sw = source.WorkArea.ToRectangle();
            var tw = target.WorkArea.ToRectangle();
            if (sw == tw)
                return bounds;

            if (sw.Width <= 0 || sw.Height <= 0)
                return KeepVisible(bounds, target);

            double fx = (double)tw.Width / sw.Width;
            double fy = (double)tw.Height / sw.Height;
            var mapped = new Rectangle(
                tw.X + (int)Math.Round((bounds.X - sw.X) * fx),
                tw.Y + (int)Math.Round((bounds.Y - sw.Y) * fy),
                Math.Max(200, (int)Math.Round(bounds.Width * fx)),
                Math.Max(150, (int)Math.Round(bounds.Height * fy)));
            return KeepVisible(mapped, target);
        }

        private static Rectangle KeepVisible(Rectangle r, MonitorSnapshot? monitor)
        {
            if (monitor == null)
                return r;

            var wa = monitor.WorkArea.ToRectangle();
            // Invisible resize borders of Windows 10/11 stick out a few pixels, allow that.
            var allowed = Rectangle.Inflate(wa, 16, 16);
            int width = Math.Min(r.Width, allowed.Width);
            int height = Math.Min(r.Height, allowed.Height);
            int x = Math.Clamp(r.X, allowed.Left, allowed.Right - width);
            int y = Math.Clamp(r.Y, allowed.Top, allowed.Bottom - height);
            return new Rectangle(x, y, width, height);
        }
    }
}
