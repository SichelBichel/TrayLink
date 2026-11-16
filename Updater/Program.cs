#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

internal static class Program
{
    // Exit codes (Host App kann das auswerten)
    // 0    = success
    // 10   = bad args
    // 20   = app did not exit in time (and kill disabled or failed)
    // 30   = msi not found
    // 40   = msiexec start failed
    // 50   = msiexec returned failure
    // 60   = restart failed
    // 70   = app path missing for restart

    public static int Main(string[] args)
    {
        try
        {
            var opt = Options.Parse(args);

            if (!File.Exists(opt.MsiPath))
            {
                Console.Error.WriteLine($"MSI not found: {opt.MsiPath}");
                return 30;
            }

            if (opt.WaitPid is int pid)
            {
                if (!WaitForProcessExit(pid, opt.WaitSeconds))
                {
                    if (opt.KillIfRunning)
                    {
                        if (!TryKillProcessTree(pid))
                        {
                            Console.Error.WriteLine($"Process PID {pid} did not exit within {opt.WaitSeconds}s and kill failed.");
                            return 20;
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine($"Process PID {pid} did not exit within {opt.WaitSeconds}s.");
                        return 20;
                    }
                }
            }

            var msiexecArgs = BuildMsiExecArgs(opt);
            Console.WriteLine($"Running: msiexec.exe {msiexecArgs}");

            var exit = RunProcess("msiexec.exe", msiexecArgs);
            Console.WriteLine($"msiexec exit code: {exit}");

            if (exit != 0 && exit != 3010 && exit != 1641)
                return 50;

            if (opt.Restart)
            {
                if (string.IsNullOrWhiteSpace(opt.AppPath))
                    return 70;

                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = opt.AppPath!,
                        Arguments = opt.AppArgs ?? "",
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(opt.AppPath!) ?? Environment.CurrentDirectory
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Restart failed: {ex}");
                    return 60;
                }
            }

            return exit == 0 ? 0 : exit;
        }
        catch (Options.BadArgsException e)
        {
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine(Options.Usage);
            return 10;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 50;
        }
    }
    private static string BuildMsiExecArgs(Options o)
    {
        var quotedMsi = Quote(o.MsiPath);
        var ui = o.Silent ? "/qn" : "";
        var log = string.IsNullOrWhiteSpace(o.LogPath)
            ? ""
            : $"/l*v {Quote(o.LogPath!)}";

        string? installDir = null;
        if (!string.IsNullOrWhiteSpace(o.AppPath))
            installDir = Path.GetDirectoryName(o.AppPath!);

        var dirArg = string.IsNullOrWhiteSpace(installDir)
            ? ""
            : $"TARGETDIR={Quote(EnsureTrailingBackslash(installDir))}";

        return string.Join(" ", new[]
        {
            "/i", quotedMsi,
            ui,
            "/norestart",
            log,
            dirArg
        }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    private static bool WaitForProcessExit(int pid, int waitSeconds)
    {
        try
        {
            var p = Process.GetProcessById(pid);
            if (p.HasExited) return true;
            return p.WaitForExit(waitSeconds * 1000);
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    private static bool TryKillProcessTree(int pid)
    {
        try
        {
            using var p = Process.GetProcessById(pid);
            if (p.HasExited) return true;

#if NET6_0_OR_GREATER
            p.Kill(entireProcessTree: true);
#else
            p.Kill();
#endif
            p.WaitForExit(15_000);
            return p.HasExited;
        }
        catch
        {
            return false;
        }
    }

    private static int RunProcess(string file, string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = file,
            Arguments = arguments,
            UseShellExecute = true,
        };

        using var p = Process.Start(psi);
        if (p == null) return 40;
        p.WaitForExit();
        return p.ExitCode;
    }

    private static string Quote(string s)
        => s.Contains(' ') ? $"\"{s}\"" : s;

    private static string EnsureTrailingBackslash(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;
        return path.EndsWith("\\", StringComparison.Ordinal) ? path : path + "\\";
    }

    private sealed class Options
    {
        public string MsiPath { get; init; } = "";
        public bool Silent { get; init; }
        public int? WaitPid { get; init; }
        public int WaitSeconds { get; init; } = 30;
        public bool KillIfRunning { get; init; }
        public bool Restart { get; init; }
        public string? AppPath { get; init; }
        public string? AppArgs { get; init; }
        public string? LogPath { get; init; }

        public static readonly string Usage =
@"Updater.exe --msi <path> [--silent] [--wait-pid <pid>] [--wait-seconds <n>] [--kill]
            [--restart --app <path> [--app-args <args>]] [--log <msi.log>]

Examples:
  Updater.exe --msi ""C:\Temp\App.msi"" --silent --wait-pid 1234 --kill --restart --app ""C:\Program Files\App\App.exe""
  Updater.exe --msi ""C:\Temp\App.msi"" --log ""C:\Temp\msi.log""";

        public sealed class BadArgsException : Exception
        {
            public BadArgsException(string msg) : base(msg) { }
        }

        public static Options Parse(string[] args)
        {
            string? Get(string name)
            {
                var i = Array.IndexOf(args, name);
                if (i < 0) return null;
                if (i + 1 >= args.Length)
                    throw new BadArgsException($"Missing value for {name}");
                return args[i + 1];
            }

            bool Has(string name)
                => args.Contains(name, StringComparer.OrdinalIgnoreCase);

            var msi = Get("--msi") ?? throw new BadArgsException("Missing --msi");

            int? pid = null;
            var pidStr = Get("--wait-pid");
            if (pidStr != null && int.TryParse(pidStr, out var p))
                pid = p;

            var ws = 30;
            var wsStr = Get("--wait-seconds");
            if (wsStr != null && int.TryParse(wsStr, out var wsv))
                ws = Math.Clamp(wsv, 1, 600);

            var restart = Has("--restart");
            var app = Get("--app");
            var appArgs = Get("--app-args");

            if (restart && string.IsNullOrWhiteSpace(app))
                throw new BadArgsException("Missing --app (required when --restart is set)");

            return new Options
            {
                MsiPath = msi,
                Silent = Has("--silent"),
                WaitPid = pid,
                WaitSeconds = ws,
                KillIfRunning = Has("--kill"),
                Restart = restart,
                AppPath = app,
                AppArgs = appArgs,
                LogPath = Get("--log")
            };
        }
    }
}
