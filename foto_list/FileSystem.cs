using System.IO;
using System.Linq;

namespace foto_list
{
    public class FileSystem : IFileSystem
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);
        
        public string GetFullPath(string path) => Path.GetFullPath(path);
        
        public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);
        
        public string[] GetDirectories(string path) => Directory.GetDirectories(path);
        
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        
        public bool FileExists(string path) => File.Exists(path);
        
        public StreamReader OpenText(string path) => File.OpenText(path);
        
        public StreamWriter CreateText(string path) => File.CreateText(path);
        
        public string Combine(params string[] paths)
        {
            // Ensure paths are normalized for the current OS
            var normalizedPaths = paths.Select(p => p.Replace('\\', Path.DirectorySeparatorChar)
                                                   .Replace('/', Path.DirectorySeparatorChar))
                                     .ToArray();
            return Path.Combine(normalizedPaths);
        }
        
        public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);
        
        public string GetFileName(string path) => Path.GetFileName(path);
        
        public string GetExtension(string path) => Path.GetExtension(path);
    }
} 