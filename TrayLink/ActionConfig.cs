namespace TrayLink
{
    public class ActionConfig
    {
        public const string TypeUrl = "URL";
        public const string TypeFile = "FILE";
        public const string TypePath = "PATH";

        public static readonly string[] Types = { TypeUrl, TypeFile, TypePath };

        public string ActionName { get; set; } = "New Action";
        public string ActionType { get; set; } = TypeUrl;
        public string PathOrUrl { get; set; } = "";

        public ActionConfig Clone() => new()
        {
            ActionName = ActionName,
            ActionType = ActionType,
            PathOrUrl = PathOrUrl
        };

        /// <summary>Builds an entry for a file or folder (shell integration, drag+drop from Explorer).</summary>
        public static ActionConfig FromPath(string path)
        {
            bool isDir = Directory.Exists(path);
            string name = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrWhiteSpace(name))
                name = path; // drive root like C:\

            return new ActionConfig
            {
                ActionName = isDir ? name + " (Folder)" : name,
                ActionType = isDir ? TypePath : TypeFile,
                PathOrUrl = path
            };
        }
    }
}
