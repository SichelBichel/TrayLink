#nullable enable
using TrayLink.Updater;
using System;
using System.Reflection;

namespace TrayLink.Updater;

internal static class VersionUtil
{
    public static string GetInstalledVersion()
    {
        var asm = Assembly.GetExecutingAssembly();

        var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (!string.IsNullOrWhiteSpace(info))
            return info.Trim();

        var file = asm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        if (!string.IsNullOrWhiteSpace(file))
            return file.Trim();

        return "0.0.0";
    }

    public static string StripBuildMeta(string v)
    {
        if (string.IsNullOrWhiteSpace(v))
            return "0.0.0";

        var s = v.Trim();
        var plus = s.IndexOf('+');
        return plus >= 0 ? s[..plus] : s;
    }
}
