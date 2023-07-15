using System;
using System.Collections.Specialized;
using System.IO;

namespace foto_list
{
    public class FotoManager : IFotoManger
    {
        public FotoManager(){
        }

        public string CreateListFile(string listFileName)
        {
            var path = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(path))
                return ConstDef.ConstErrFotoPath;


            var fullPath = Path.GetFullPath(path);

            Console.WriteLine(fullPath);

            StringCollection allFiles = new StringCollection();
            listAllFiles(allFiles, fullPath, "*.*", true);

            return WriteListFile(listFileName, allFiles);     
        }

        public string GenerateDiffReports(string listFileName)
        {
            var path = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(path))
                return ConstDef.ConstErrFotoPath;
            var fullPath = Path.GetFullPath(path);

            Console.WriteLine(fullPath);

            StringCollection allPhotosInBaseline = new StringCollection();

            if (!ReadListInFile(listFileName, allPhotosInBaseline))
                return ConstDef.ConstErrFotolistFile;

            StringCollection allFilesInTarget = new StringCollection();
            
            listAllFiles(allFilesInTarget, fullPath, "*.*", true);

            StringCollection allMissingFileInTarget = new StringCollection();

            StringCollection allMissingFileInBaseline = new StringCollection();

            foreach (var name in allPhotosInBaseline)
            {
                if (!allFilesInTarget.Contains(name))
                    allMissingFileInTarget.Add(name);
            }


            foreach (var name in allFilesInTarget)
            {
                if (!allPhotosInBaseline.Contains(name))
                    allMissingFileInBaseline.Add(name);
            }
            var baselineDiffFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ConstDef.ConstBaselineDiffFileName);

            var targetDiffFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ConstDef.ConstTargetDiffFileName);

            var result = WriteListFile(baselineDiffFileName, allMissingFileInBaseline);
            result += "  " + WriteListFile(targetDiffFileName, allMissingFileInTarget);

            return result;

        }

        public string CleanPhoto(string listFileName, string reportFileName)
        {
            var path = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(path))
                return ConstDef.ConstErrFotoPath;
            

            var fullPath = Path.GetFullPath(path);

            Console.WriteLine(fullPath);

            StringCollection allPhotos = new StringCollection();

            if (!ReadListInFile(listFileName, allPhotos))
                return ConstDef.ConstErrFotolistFile;

            StringCollection removedFiles = new StringCollection();

            cleanAllFiles(allPhotos, removedFiles, fullPath, "*.*", true);
            var removedFileReport = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ConstDef.ConstRemovedFileName);
            return WriteListFile(removedFileReport, removedFiles);
        }

        protected virtual string InputPhotoFolderPath()
        {
            Console.WriteLine(ConstDef.ConstFotoPath);
            var path = Console.ReadLine();
            if (!Directory.Exists(path))
            {
                Console.WriteLine(ConstDef.ConstErrFotoPath);
                Console.WriteLine("The input Path is: {0}", path);
                return string.Empty;
            }
            return path;
        }

        protected virtual bool ReadListInFile(string listFileName, StringCollection allPhotos)
        {
            bool re = true;
            try
            {
                using (StreamReader listFileReader = new StreamReader(listFileName))
                {

                    while (!listFileReader.EndOfStream)
                    {
                        var line = listFileReader.ReadLine();
                        allPhotos.Add(line);
                    }
                }
            }
            catch
            {
                re = false;
            }
                
            return re;
        }

        protected virtual string WriteListFile(string fileName, StringCollection allFiles)
        {
            string message;
            try { 
                using (StreamWriter outputFile = new StreamWriter(fileName))
                {
                    foreach (var name in allFiles)
                    {
                        outputFile.WriteLine(name);
                    }
                }
                Console.WriteLine("Totally: {0} photos' info have been added into list file {1} .", allFiles.Count, fileName);

                message = ConstDef.ConstMesgReturnList + fileName;

            }
            catch
            {
                message = ConstDef.ConstErrWriteFile + fileName;
            }
            Console.WriteLine(message);
            return message;
        }

        private static StringCollection cleanAllFiles(StringCollection allPhotos, StringCollection removedFiles, string path, string ext, bool scanDirOk)
        {
            var cleanPath = Path.Combine(path, ConstDef.ConstTempRemoveFolderName);
            string[] listFilesCurrDir = Directory.GetFiles(path, ext);
            if(!Directory.Exists(cleanPath))
                Directory.CreateDirectory(cleanPath);

            // read the array 'listFilesCurrDir'
            foreach (string rowFile in listFilesCurrDir)
            {
                FileInfo fileInfo = new FileInfo(rowFile);
                var name = fileInfo.Name.Replace(fileInfo.Extension, "");
                // If the file is not already in the 'allFiles' list
                if (allPhotos.Contains(name) == false)
                {
                    // Add the file (at least its address) to 'allFiles'
                    var newPath = Path.Combine(cleanPath, fileInfo.Name);
                    fileInfo.MoveTo(newPath);
                    removedFiles.Add(rowFile);
                }
            }
            // Clear the 'listFilesCurrDir' table for the next list of subfolders
            listFilesCurrDir = null;

            // If you allow subfolders to be read
            if (scanDirOk)
            {
                // List all the subfolders present in the 'path'
                string[] listDirCurrDir = Directory.GetDirectories(path);

                // if there are subfolders (if the list is not empty)
                if (listDirCurrDir.Length != 0)
                {
                    // read the array 'listDirCurrDir'
                    foreach (string rowDir in listDirCurrDir)
                    {
                        if (!rowDir.Contains(ConstDef.ConstTempRemoveFolderName))
                            // Restart the procedure to scan each subfolder
                            cleanAllFiles(allPhotos, removedFiles, rowDir, ext, scanDirOk);
                    }
                }
                // Clear the 'listDirCurrDir' table for the next list of subfolders
                listDirCurrDir = null;

            }
            // return 'allFiles'
            return removedFiles;
        }

        private static StringCollection listAllFiles(StringCollection allFiles, string path, string ext, bool scanDirOk)
        {
            // listFilesCurrDir: Table containing the list of files in the 'path' folder
            string[] listFilesCurrDir = Directory.GetFiles(path, ext);

            // read the array 'listFilesCurrDir'
            foreach (string rowFile in listFilesCurrDir)
            {
                FileInfo fileInfo = new FileInfo(rowFile);
                var name = fileInfo.Name.Replace(fileInfo.Extension, "");
                // If the file is not already in the 'allFiles' list
                if (allFiles.Contains(name) == false)
                {
                    // Add the file (at least its address) to 'allFiles'
                    allFiles.Add(name);
                }
            }
            // Clear the 'listFilesCurrDir' table for the next list of subfolders
            listFilesCurrDir = null;

            // If you allow subfolders to be read
            if (scanDirOk)
            {
                // List all the subfolders present in the 'path'
                string[] listDirCurrDir = Directory.GetDirectories(path);

                // if there are subfolders (if the list is not empty)
                if (listDirCurrDir.Length != 0)
                {
                    // read the array 'listDirCurrDir'
                    foreach (string rowDir in listDirCurrDir)
                    {
                        // Restart the procedure to scan each subfolder
                        listAllFiles(allFiles, rowDir, ext, scanDirOk);
                    }
                }
                // Clear the 'listDirCurrDir' table for the next list of subfolders
                listDirCurrDir = null;

            }
            // return 'allFiles'
            return allFiles;
        }


    }
}

