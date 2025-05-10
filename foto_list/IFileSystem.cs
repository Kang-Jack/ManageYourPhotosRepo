using System.Collections.Specialized;
using System.IO;

namespace foto_list
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        string GetFullPath(string path);
        string[] GetFiles(string path, string searchPattern);
        string[] GetDirectories(string path);
        void CreateDirectory(string path);
        bool FileExists(string path);
        StreamReader OpenText(string path);
        StreamWriter CreateText(string path);
        string Combine(params string[] paths);
        string GetFileNameWithoutExtension(string path);
        string GetFileName(string path);
        string GetExtension(string path);
        void MoveFile(string sourcePath, string targetPath);
    }
} 