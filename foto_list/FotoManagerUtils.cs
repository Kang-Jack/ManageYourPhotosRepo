using System;
using System.Collections.Specialized;

namespace foto_list;

public class FotoManagerUtils
{
    public static StringCollection sortByName(StringCollection orgCollection)
    {
        if (orgCollection == null || orgCollection.Count < 2)
            return orgCollection;
        String[] sortedArry = new string[orgCollection.Count];
        orgCollection.CopyTo(sortedArry, 0);
        Array.Sort(sortedArry);
        orgCollection.Clear();
        orgCollection.AddRange(sortedArry);
        return orgCollection;

    }

    public static StringCollection listAllFiles(IFileSystem fileSystem, StringCollection allFiles, string path, string ext, bool scanDirOk)
    {
        string[] listFilesCurrDir = fileSystem.GetFiles(path, ext);

        foreach (string rowFile in listFilesCurrDir)
        {
            var name = fileSystem.GetFileNameWithoutExtension(rowFile);
            if ((!name.StartsWith('.')) && allFiles.Contains(name) == false)
            {
                allFiles.Add(name);
            }
        }

        if (scanDirOk)
        {
            string[] listDirCurrDir = fileSystem.GetDirectories(path);

            if (listDirCurrDir.Length != 0)
            {
                foreach (string rowDir in listDirCurrDir)
                {
                    listAllFiles(fileSystem, allFiles, rowDir, ext, scanDirOk);
                }
            }
        }
        return sortByName(allFiles);
    }

    public static string checkFileName(IFileSystem fileSystem, string fullPathFile,string constFileName, string prefix)
    {
        if (string.IsNullOrEmpty(fullPathFile))
            return fullPathFile;
        if (!fileSystem.FileExists(fullPathFile))
            return fullPathFile;
        var newFileNameFullPath = prefix + constFileName;
        return fullPathFile.Replace(constFileName, newFileNameFullPath);
    }
}