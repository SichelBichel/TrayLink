using TrayLink.Core;

namespace TrayLink
{
    /// <summary>
    /// Light or dark page content (Options > Appearance): System follows the Windows setting "app mode".
    /// Windows Forms can switch the colour mode only once, before the first window: a change needs a restart.
    /// </summary>
    internal static class ColorModeSupport
    {
        public const string System = "System";
        public const string Light = "Light";
        public const string Dark = "Dark";

        public static void Apply(string? mode)
        {
#pragma warning disable WFO5001 // experimental in .NET 9, kept for compatibility (same as AutoBak Server)
            try
            {
                var colorMode = mode switch
                {
                    Dark => SystemColorMode.Dark,
                    Light => SystemColorMode.Classic,
                    _ => SystemColorMode.System
                };
                Application.SetColorMode(colorMode);
                Theme.IsDark = Application.IsDarkModeEnabled;
            }
            catch
            {
                Theme.IsDark = false; // light is always possible
            }
#pragma warning restore WFO5001
        }

        public static string Current => AppSettings.Current.ColorMode switch
        {
            Dark => Dark,
            Light => Light,
            _ => System
        };
    }
}
