using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shared.Vfs
{
    public static class VfsManager
    {
        private static readonly Dictionary<string, string> _vfsIndex = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> _vfsDirIndex = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly object _lock = new object();
        private static bool _initialized = false;

        static VfsManager()
        {
            Initialize(Directory.GetCurrentDirectory());
        }

        public static void Initialize(string rootDir = null)
        {
            lock (_lock)
            {
                if (_initialized) return;
                if (string.IsNullOrEmpty(rootDir))
                {
                    rootDir = Directory.GetCurrentDirectory();
                }

                try
                {
                    if (!Directory.Exists(rootDir)) return;

                    // Index directories
                    var dirs = Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories);
                    foreach (var dir in dirs)
                    {
                        var normalized = Path.GetFullPath(dir).Replace('\\', '/');
                        _vfsDirIndex[normalized] = dir;
                    }

                    // Index files
                    var files = Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var normalized = Path.GetFullPath(file).Replace('\\', '/');
                        _vfsIndex[normalized] = file;
                    }

                    _initialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VFS] Indexing error: {ex}");
                }
            }
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            return path.Replace('\\', '/');
        }

        public static string Resolve(string path) => ResolvePath(path, false);

        public static string ResolveDir(string path) => ResolvePath(path, true);

        private static string ResolvePath(string path, bool directory)
        {
            if (string.IsNullOrEmpty(path)) return path;
            var fullPath = Path.GetFullPath(NormalizePath(path));
            var index = directory ? _vfsDirIndex : _vfsIndex;
            bool Exists(string candidate) => directory ? Directory.Exists(candidate) : File.Exists(candidate);

            // Prefer the exact name, and never return stale cached paths after deletion.
            if (Exists(fullPath)) return fullPath;
            lock (_lock)
            {
                if (index.TryGetValue(fullPath, out var cached) && Exists(cached))
                    return cached;
                index.Remove(fullPath);
            }

            var parent = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(parent)) return fullPath;
            var resolvedParent = ResolveDir(parent);
            var name = Path.GetFileName(fullPath);
            var resolved = Path.Combine(resolvedParent, name);
            if (Directory.Exists(resolvedParent))
            {
                var entries = directory
                    ? Directory.EnumerateDirectories(resolvedParent)
                    : Directory.EnumerateFiles(resolvedParent);
                foreach (var entry in entries)
                {
                    if (!string.Equals(Path.GetFileName(entry), name, StringComparison.OrdinalIgnoreCase))
                        continue;
                    lock (_lock) index[fullPath] = entry;
                    return entry;
                }
            }

            // New files still need normalized separators and the existing parent's casing.
            return resolved;
        }

        public static bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var resolved = Resolve(path);
            return File.Exists(resolved);
        }

        public static bool DirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var resolved = ResolveDir(path);
            return Directory.Exists(resolved);
        }

        public static FileStream OpenRead(string path)
        {
            var resolved = Resolve(path);
            return File.OpenRead(resolved);
        }

        public static byte[] ReadAllBytes(string path)
        {
            var resolved = Resolve(path);
            return File.ReadAllBytes(resolved);
        }

        public static string[] ReadAllLines(string path)
        {
            var resolved = Resolve(path);
            return File.ReadAllLines(resolved);
        }

        public static string ReadAllText(string path)
        {
            var resolved = Resolve(path);
            return File.ReadAllText(resolved);
        }

        public static void WriteAllText(string path, string contents)
        {
            var resolved = Resolve(path);
            File.WriteAllText(resolved, contents);
            RegisterFile(path, resolved);
        }

        public static void WriteAllText(string path, string contents, System.Text.Encoding encoding)
        {
            var resolved = Resolve(path);
            File.WriteAllText(resolved, contents, encoding);
            RegisterFile(path, resolved);
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents)
        {
            var resolved = Resolve(path);
            File.WriteAllLines(resolved, contents);
            RegisterFile(path, resolved);
        }

        public static void WriteAllLines(string path, IEnumerable<string> contents, System.Text.Encoding encoding)
        {
            var resolved = Resolve(path);
            File.WriteAllLines(resolved, contents, encoding);
            RegisterFile(path, resolved);
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            var resolved = Resolve(path);
            File.WriteAllBytes(resolved, bytes);
            RegisterFile(path, resolved);
        }

        public static DateTime GetLastWriteTime(string path)
        {
            var resolved = Resolve(path);
            return File.GetLastWriteTime(resolved);
        }

        public static StreamWriter AppendText(string path)
        {
            var resolved = Resolve(path);
            return File.AppendText(resolved);
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            var resolved = Resolve(path);
            return File.ReadLines(resolved);
        }

        public static void RegisterFile(string path, string resolvedPath)
        {
            if (string.IsNullOrEmpty(path)) return;
            var normalized = Path.GetFullPath(NormalizePath(path)).Replace('\\', '/');
            lock (_lock)
            {
                _vfsIndex[normalized] = resolvedPath;
            }
        }

        public static void UnregisterFile(string path, string resolvedPath = null)
        {
            if (string.IsNullOrEmpty(path)) return;
            var normalized = Path.GetFullPath(NormalizePath(path)).Replace('\\', '/');
            lock (_lock)
            {
                _vfsIndex.Remove(normalized);
                if (resolvedPath != null)
                {
                    var resolvedNormalized = Path.GetFullPath(NormalizePath(resolvedPath)).Replace('\\', '/');
                    _vfsIndex.Remove(resolvedNormalized);
                }
            }
        }

        public static void RegisterDir(string path, string resolvedPath)
        {
            if (string.IsNullOrEmpty(path)) return;
            var normalized = Path.GetFullPath(NormalizePath(path)).Replace('\\', '/');
            lock (_lock)
            {
                _vfsDirIndex[normalized] = resolvedPath;
            }
        }

        public static void UnregisterDir(string path, string resolvedPath = null)
        {
            if (string.IsNullOrEmpty(path)) return;
            var normalized = Path.GetFullPath(NormalizePath(path)).Replace('\\', '/');
            lock (_lock)
            {
                _vfsDirIndex.Remove(normalized);
                if (resolvedPath != null)
                {
                    var resolvedNormalized = Path.GetFullPath(NormalizePath(resolvedPath)).Replace('\\', '/');
                    _vfsDirIndex.Remove(resolvedNormalized);
                }
            }
        }

        public static Regex ConvertGlobToRegex(string wildcard, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(wildcard))
            {
                return new Regex("^.*$");
            }
            string regexPattern = "^" + Regex.Escape(wildcard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
            return new Regex(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public static string[] GetFilesMatching(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var resolvedDir = ResolveDir(path);
            if (!Directory.Exists(resolvedDir)) return Array.Empty<string>();
            var regex = ConvertGlobToRegex(searchPattern);
            // Enumerate the filesystem so files created by FileStream or external tools
            // are visible, and deleted files cannot remain in directory listings.
            return Directory.EnumerateFiles(resolvedDir, "*", searchOption)
                .Where(file => regex.IsMatch(Path.GetFileName(file))).ToArray();
        }

        public static string[] GetDirectoriesMatching(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var resolvedDir = ResolveDir(path);
            if (!Directory.Exists(resolvedDir)) return Array.Empty<string>();
            var regex = ConvertGlobToRegex(searchPattern);
            return Directory.EnumerateDirectories(resolvedDir, "*", searchOption)
                .Where(dir => regex.IsMatch(Path.GetFileName(dir))).ToArray();
        }
    }
}
