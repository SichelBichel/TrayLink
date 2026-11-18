using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrayLink.Core.Layouts
{
    /// <summary>One JSON file per layout in %APPDATA%\TrayLink\layouts.</summary>
    internal static class LayoutStore
    {
        public static string Folder => Path.Combine(AppPaths.DataDir, "layouts");

        private static readonly JsonSerializerOptions Json = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static List<DesktopLayout> LoadAll()
        {
            var list = new List<DesktopLayout>();
            if (!Directory.Exists(Folder))
                return list;

            foreach (var file in Directory.GetFiles(Folder, "*.json"))
            {
                try
                {
                    var layout = JsonSerializer.Deserialize<DesktopLayout>(File.ReadAllText(file), Json);
                    if (layout != null && !string.IsNullOrWhiteSpace(layout.Name))
                        list.Add(layout);
                }
                catch
                {
                    // a broken file is skipped, not fatal
                }
            }

            return list.OrderBy(l => l.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
        }

        public static DesktopLayout? Load(string name) =>
            LoadAll().FirstOrDefault(l => l.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

        public static void Save(DesktopLayout layout)
        {
            Directory.CreateDirectory(Folder);
            var path = PathOf(layout.Name);
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(layout, Json), new UTF8Encoding(false));
            File.Move(tmp, path, overwrite: true);
        }

        public static void Delete(string name)
        {
            var path = PathOf(name);
            if (File.Exists(path))
                File.Delete(path);
        }

        public static void Rename(DesktopLayout layout, string newName)
        {
            var oldName = layout.Name;
            layout.Name = newName;
            Save(layout);
            if (!PathOf(oldName).Equals(PathOf(newName), StringComparison.OrdinalIgnoreCase))
                Delete(oldName);
        }

        public static bool Exists(string name) => File.Exists(PathOf(name));

        private static string PathOf(string name)
        {
            var safe = new string(name.Trim().Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c).ToArray());
            if (string.IsNullOrWhiteSpace(safe))
                safe = "Layout";
            return Path.Combine(Folder, safe + ".json");
        }
    }
}
