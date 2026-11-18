using System.Runtime.InteropServices;

namespace TrayLink.Core.Layouts
{
    /// <summary>
    /// Folders of the open File Explorer windows through the Shell.Application COM object.
    /// Windows 11 lists every tab as its own entry with the handle of its window.
    /// Must be called on an STA thread.
    /// </summary>
    internal static class ExplorerWindows
    {
        public static Dictionary<IntPtr, List<string>> GetFolders()
        {
            var result = new Dictionary<IntPtr, List<string>>();
            object? shell = null;
            object? windows = null;

            try
            {
                var type = Type.GetTypeFromProgID("Shell.Application");
                if (type == null)
                    return result;

                shell = Activator.CreateInstance(type);
                if (shell == null)
                    return result;

                windows = ((dynamic)shell).Windows();
                int count = ((dynamic)windows!).Count;

                for (int i = 0; i < count; i++)
                {
                    object? item = null;
                    try
                    {
                        item = ((dynamic)windows).Item(i);
                        if (item == null)
                            continue;

                        dynamic w = item;
                        string fullName = Convert.ToString(w.FullName) ?? "";
                        if (!fullName.EndsWith("explorer.exe", StringComparison.OrdinalIgnoreCase))
                            continue; // old Internet Explorer windows are in the same list

                        var hwnd = new IntPtr(Convert.ToInt64(w.HWND));
                        string? path = ReadPath(w);
                        if (string.IsNullOrWhiteSpace(path))
                            continue;

                        if (!result.TryGetValue(hwnd, out var list))
                            result[hwnd] = list = new List<string>();
                        if (!list.Contains(path, StringComparer.OrdinalIgnoreCase))
                            list.Add(path);
                    }
                    catch
                    {
                        // a window that closes while we read it
                    }
                    finally
                    {
                        Release(item);
                    }
                }
            }
            catch
            {
                // no shell available (unusual), Explorer windows are then restored by position only
            }
            finally
            {
                Release(windows);
                Release(shell);
            }

            return result;
        }

        private static string? ReadPath(dynamic w)
        {
            try
            {
                // Real folders and virtual ones (This PC = ::{20D04FE0-...}) both have a path here.
                string? path = Convert.ToString(w.Document.Folder.Self.Path);
                if (!string.IsNullOrWhiteSpace(path))
                    return path;
            }
            catch
            {
                // fall back to the URL
            }

            try
            {
                string url = Convert.ToString(w.LocationURL) ?? "";
                if (url.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                    return new Uri(url).LocalPath;
            }
            catch
            {
                // ignore
            }
            return null;
        }

        private static void Release(object? com)
        {
            try
            {
                if (com != null && Marshal.IsComObject(com))
                    Marshal.ReleaseComObject(com);
            }
            catch
            {
                // ignore
            }
        }
    }
}
