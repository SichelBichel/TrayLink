using System.Text;

namespace TrayLink.Core
{
    /// <summary>
    /// Reads and writes actions.ini. The order of the sections is the order of the cards.
    /// </summary>
    internal static class ActionStore
    {
        public static List<ActionConfig> CreateDefault() => new()
        {
            new ActionConfig { ActionName = "Open Notepad", ActionType = ActionConfig.TypeFile, PathOrUrl = "notepad.exe" }
        };

        /// <summary>Loads the actions. Creates a default file when none exists.</summary>
        public static List<ActionConfig> Load(out string rawText)
        {
            AppPaths.EnsureDataDir();

            if (!File.Exists(AppPaths.ActionsFile))
            {
                var defaults = CreateDefault();
                rawText = Save(defaults);
                return defaults;
            }

            rawText = ReadAllTextShared(AppPaths.ActionsFile);
            return Parse(rawText);
        }

        public static List<ActionConfig> Parse(string text)
        {
            var result = new List<ActionConfig>();
            ActionConfig? current = null;

            using var reader = new StringReader(text);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var trimmed = line.Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith(';') || trimmed.StartsWith('#'))
                    continue;

                if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
                {
                    if (current != null)
                        result.Add(current);
                    current = new ActionConfig { ActionName = "", PathOrUrl = "" };
                    continue;
                }

                if (current == null)
                    continue;

                int eq = trimmed.IndexOf('=');
                if (eq <= 0)
                    continue;

                var key = trimmed[..eq].Trim().ToLowerInvariant();
                var value = trimmed[(eq + 1)..].Trim();

                switch (key)
                {
                    case "actionname": current.ActionName = value; break;
                    case "actiontype": current.ActionType = value.ToUpperInvariant(); break;
                    case "pathorurl": current.PathOrUrl = value; break;
                }
            }

            if (current != null)
                result.Add(current);

            // Drop empty sections, give unnamed entries a readable name.
            result.RemoveAll(a => string.IsNullOrWhiteSpace(a.PathOrUrl) && string.IsNullOrWhiteSpace(a.ActionName));
            foreach (var a in result)
            {
                if (string.IsNullOrWhiteSpace(a.ActionName))
                    a.ActionName = a.PathOrUrl;
                if (string.IsNullOrWhiteSpace(a.ActionType))
                    a.ActionType = ActionConfig.TypeUrl;
            }

            return result;
        }

        public static string Serialize(IReadOnlyList<ActionConfig> actions)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < actions.Count; i++)
            {
                sb.AppendLine($"[Action{i + 1}]");
                sb.AppendLine($"ActionName={Sanitize(actions[i].ActionName)}");
                sb.AppendLine($"ActionType={Sanitize(actions[i].ActionType)}");
                sb.AppendLine($"PathOrUrl={Sanitize(actions[i].PathOrUrl)}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>Writes atomically (temp file + move) and returns the written text.</summary>
        public static string Save(IReadOnlyList<ActionConfig> actions)
        {
            return WithWriteLock(() => SaveUnlocked(actions));
        }

        /// <summary>
        /// Appends entries to the stored list (used by the Explorer context menu).
        /// With a multi selection Explorer starts one process per item, so this runs under a
        /// cross process lock, otherwise entries would overwrite each other.
        /// </summary>
        public static void Append(IEnumerable<ActionConfig> newEntries)
        {
            WithWriteLock(() =>
            {
                var list = File.Exists(AppPaths.ActionsFile)
                    ? Parse(ReadAllTextShared(AppPaths.ActionsFile))
                    : CreateDefault();
                list.AddRange(newEntries);
                return SaveUnlocked(list);
            });
        }

        private static string SaveUnlocked(IReadOnlyList<ActionConfig> actions)
        {
            AppPaths.EnsureDataDir();
            var text = Serialize(actions);
            var tmp = AppPaths.ActionsFile + "." + Environment.ProcessId + ".tmp";

            File.WriteAllText(tmp, text, new UTF8Encoding(false));
            File.Move(tmp, AppPaths.ActionsFile, overwrite: true);
            return text;
        }

        private static T WithWriteLock<T>(Func<T> action)
        {
            using var mutex = new Mutex(false, @"Local\ReHoGa.TrayLink.ConfigWrite");
            bool owned = false;
            try
            {
                try
                {
                    owned = mutex.WaitOne(TimeSpan.FromSeconds(10));
                }
                catch (AbandonedMutexException)
                {
                    owned = true; // previous owner crashed, we own it now
                }
                return action();
            }
            finally
            {
                if (owned)
                    mutex.ReleaseMutex();
            }
        }

        public static string ReadAllTextShared(string path)
        {
            // The file can be written by another TrayLink process at the same moment.
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    using var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                    return sr.ReadToEnd();
                }
                catch (IOException) when (attempt < 5)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private static string Sanitize(string? value) =>
            (value ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
    }
}
