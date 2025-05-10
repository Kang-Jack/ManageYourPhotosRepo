using System;
using System.Collections.Specialized;
using System.IO;

namespace foto_list
{
    public class FotoManager : IFotoManger
    {
        private readonly IFileSystem _fileSystem;

        public FotoManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public string CreateListFile(string listFileName)
        {
            var path = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(path))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(path);
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
            var fullPath = _fileSystem.GetFullPath(path);

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

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var baselineDiffFileName = _fileSystem.Combine(desktopPath, ConstDef.ConstBaselineDiffFileName);
            var targetDiffFileName = _fileSystem.Combine(desktopPath, ConstDef.ConstTargetDiffFileName);

            var result = WriteListFile(baselineDiffFileName, allMissingFileInBaseline);
            result += "  " + WriteListFile(targetDiffFileName, allMissingFileInTarget);

            return result;
        }

        public string CleanPhoto(string listFileName, string reportFileName)
        {
            var path = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(path))
                return ConstDef.ConstErrFotoPath;

            var fullPath = _fileSystem.GetFullPath(path);
            Console.WriteLine(fullPath);

            StringCollection allPhotos = new StringCollection();

            if (!ReadListInFile(listFileName, allPhotos))
                return ConstDef.ConstErrFotolistFile;

            StringCollection removedFiles = new StringCollection();

            cleanAllFiles(allPhotos, removedFiles, fullPath, "*.*", true);
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var removedFileReport = _fileSystem.Combine(desktopPath, ConstDef.ConstRemovedFileName);
            return WriteListFile(removedFileReport, removedFiles);
        }

        protected virtual string InputPhotoFolderPath()
        {
            Console.WriteLine(ConstDef.ConstFotoPath);
            var path = Console.ReadLine();
            if (!_fileSystem.DirectoryExists(path))
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
                using (var listFileReader = _fileSystem.OpenText(listFileName))
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
            try 
            { 
                using (var outputFile = _fileSystem.CreateText(fileName))
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

        private StringCollection cleanAllFiles(StringCollection allPhotos, StringCollection removedFiles, string path, string ext, bool scanDirOk)
        {
            var cleanPath = _fileSystem.Combine(path, ConstDef.ConstTempRemoveFolderName);
            string[] listFilesCurrDir = _fileSystem.GetFiles(path, ext);
            
            if(!_fileSystem.DirectoryExists(cleanPath))
                _fileSystem.CreateDirectory(cleanPath);

            foreach (string rowFile in listFilesCurrDir)
            {
                var fileName = _fileSystem.GetFileName(rowFile);
                var name = _fileSystem.GetFileNameWithoutExtension(rowFile);
                
                if (allPhotos.Contains(name) == false)
                {
                    var newPath = _fileSystem.Combine(cleanPath, fileName);
                    File.Move(rowFile, newPath);
                    removedFiles.Add(rowFile);
                }
            }

            if (scanDirOk)
            {
                string[] listDirCurrDir = _fileSystem.GetDirectories(path);

                if (listDirCurrDir.Length != 0)
                {
                    foreach (string rowDir in listDirCurrDir)
                    {
                        if (!rowDir.Contains(ConstDef.ConstTempRemoveFolderName))
                            cleanAllFiles(allPhotos, removedFiles, rowDir, ext, scanDirOk);
                    }
                }
            }
            return removedFiles;
        }

        private StringCollection listAllFiles(StringCollection allFiles, string path, string ext, bool scanDirOk)
        {
            string[] listFilesCurrDir = _fileSystem.GetFiles(path, ext);

            foreach (string rowFile in listFilesCurrDir)
            {
                var name = _fileSystem.GetFileNameWithoutExtension(rowFile);
                if (allFiles.Contains(name) == false)
                {
                    allFiles.Add(name);
                }
            }

            if (scanDirOk)
            {
                string[] listDirCurrDir = _fileSystem.GetDirectories(path);

                if (listDirCurrDir.Length != 0)
                {
                    foreach (string rowDir in listDirCurrDir)
                    {
                        listAllFiles(allFiles, rowDir, ext, scanDirOk);
                    }
                }
            }
            return allFiles;
        }
    }
}

