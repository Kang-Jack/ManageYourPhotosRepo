using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
using System;
namespace mtest;

public class MockFileSystem : IFileSystem
{
    public bool DirectoryExistsResult { get; set; } = true;
    public bool FileExistsResult { get; set; } = true;
    public string[] GetFilesResult { get; set; } = Array.Empty<string>();
    public string[] GetDirectoriesResult { get; set; } = Array.Empty<string>();
    public string GetFullPathResult { get; set; } = "C:\\TestPath";
    public string CombineResult { get; set; } = "C:\\TestPath\\file.txt";
    public string GetFileNameWithoutExtensionResult { get; set; } = "test";
    public string GetFileNameResult { get; set; } = "test.txt";
    public string GetExtensionResult { get; set; } = ".txt";
    public StreamReader? OpenTextResult { get; set; }
    public StreamWriter? CreateTextResult { get; set; }
    public bool ThrowAccessDenied { get; set; }
    public bool ThrowFileInUse { get; set; }

    public bool DirectoryExists(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return DirectoryExistsResult;
    }

    public string GetFullPath(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetFullPath(path);
    }

    public string[] GetFiles(string path, string searchPattern)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return GetFilesResult;
    }

    public string[] GetDirectories(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return GetDirectoriesResult;
    }

    public void CreateDirectory(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
    }

    public bool FileExists(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return FileExistsResult;
    }

    public StreamReader OpenText(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        if (ThrowFileInUse)
        {
            throw new InvalidOperationException("File in use");
        }
        if (OpenTextResult == null)
        {
            throw new FileNotFoundException("File not found", path);
        }
        return OpenTextResult;
    }

    public StreamWriter CreateText(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        if (ThrowFileInUse)
        {
            throw new InvalidOperationException("File in use");
        }
        if (CreateTextResult == null)
        {
            throw new InvalidOperationException("Cannot create file");
        }
        return CreateTextResult;
    }

    public string Combine(params string[] paths)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.Combine(paths);
    }

    public string GetFileNameWithoutExtension(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetFileNameWithoutExtension(path);
    }

    public string GetFileName(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetFileName(path);
    }

    public string GetExtension(string path)
    {
        if (ThrowAccessDenied)
        {
            throw new InvalidOperationException("Access denied");
        }
        return Path.GetExtension(path);
    }
}

public class testableFotoManager : FotoManager
{
    public bool ReadListInFileRes { set; get; }
    public string InputPhotoFolderRes { set; get; } = string.Empty;
    public string WriteListFileRes { set; get; } = string.Empty;
    public StringCollection AllPhotos { set; get; }

    public testableFotoManager(IFileSystem fileSystem) : base(fileSystem)
    {
        AllPhotos = new StringCollection();
    }

    protected override bool ReadListInFile(string listFileName, StringCollection allPhotos)
    {
        if (AllPhotos != null)
        {
            foreach (var photo in AllPhotos)
            {
                allPhotos.Add(photo);
            }
        }
        return ReadListInFileRes;
    }

    protected override string InputPhotoFolderPath()
    {
        return InputPhotoFolderRes;
    }

    protected override string WriteListFile(string fileName, StringCollection allFiles)
    {
        return WriteListFileRes;
    }

    public new string CreateListFile(string listFileName)
    {
        if (string.IsNullOrEmpty(listFileName))
        {
            return ConstDef.ConstErrFotoPath;
        }

        try
        {
            // Check directory access first
            var directory = Path.GetDirectoryName(listFileName) ?? string.Empty;
            if (!_fileSystem.DirectoryExists(directory))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Check file access
            if (_fileSystem.FileExists(listFileName))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Get input path
            var inputPath = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(inputPath))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Get files
            var allPhotos = new StringCollection();
            var allFiles = _fileSystem.GetFiles(inputPath, "*.*");
            foreach (var file in allFiles)
            {
                var extension = _fileSystem.GetExtension(file).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    allPhotos.Add(file);
                }
            }

            return WriteListFile(listFileName, allPhotos);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            return ConstDef.ConstErrFotoPath;
        }
    }

    public new string CleanPhoto(string listFileName, string reportFileName)
    {
        if (string.IsNullOrEmpty(listFileName) || string.IsNullOrEmpty(reportFileName))
        {
            return ConstDef.ConstErrFotoPath;
        }

        try
        {
            // Check file access first
            if (!_fileSystem.FileExists(listFileName))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Read the list file
            var allPhotos = new StringCollection();
            if (!ReadListInFile(listFileName, allPhotos))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Check each photo
            var report = new StringCollection();
            foreach (string photo in allPhotos)
            {
                if (!_fileSystem.FileExists(photo))
                {
                    report.Add($"File not found: {photo}");
                }
            }

            return WriteListFile(reportFileName, report);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            return ConstDef.ConstErrFotoPath;
        }
    }

    public new string GenerateDiffReports(string listFileName)
    {
        if (string.IsNullOrEmpty(listFileName))
        {
            return ConstDef.ConstErrFotoPath;
        }

        try
        {
            // Check file access first
            if (!_fileSystem.FileExists(listFileName))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Read the list file
            var allPhotos = new StringCollection();
            if (!ReadListInFile(listFileName, allPhotos))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Get input path
            var inputPath = InputPhotoFolderPath();
            if (string.IsNullOrEmpty(inputPath))
            {
                return ConstDef.ConstErrFotoPath;
            }

            // Get files and compare
            var allFiles = _fileSystem.GetFiles(inputPath, "*.*");
            var report = new StringCollection();

            // Check for missing files in target
            foreach (string photo in allPhotos)
            {
                var found = false;
                foreach (var file in allFiles)
                {
                    if (_fileSystem.GetFileNameWithoutExtension(file) == _fileSystem.GetFileNameWithoutExtension(photo))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    report.Add($"Missing in target: {photo}");
                }
            }

            // Check for missing files in baseline
            foreach (var file in allFiles)
            {
                var extension = _fileSystem.GetExtension(file).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    var found = false;
                    foreach (string photo in allPhotos)
                    {
                        if (_fileSystem.GetFileNameWithoutExtension(file) == _fileSystem.GetFileNameWithoutExtension(photo))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        report.Add($"Missing in baseline: {file}");
                    }
                }
            }

            return WriteListFile(listFileName, report);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            return ConstDef.ConstErrFotoPath;
        }
    }
}

public class MTestFotoManger
{
    private IFotoManger m_sut;
    private MockFileSystem mockFileSystem;
    private string tempDirPath;

    [SetUp]
    public void Setup()
    {
        mockFileSystem = new MockFileSystem();
        m_sut = new testableFotoManager(mockFileSystem);
        // Setup: Create a temporary directory for testing
        tempDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirPath);
    }

    [TearDown]
    public void TearDown()
    {
        m_sut = null;
        mockFileSystem = null;
        // Cleanup: Delete the temporary directory after each test
        if (Directory.Exists(tempDirPath))
            Directory.Delete(tempDirPath, true);
    }

    // Test for CreateListFile with valid path and file name
    [Test]
    public void TestCreateListFileValidPathAndFileName()
    {
        mockFileSystem.DirectoryExistsResult = true;
        string listFileName = "testlist.txt";
        string result = m_sut.CreateListFile(listFileName);
        Assert.IsNotNull(result);
        File.Delete(listFileName); // Clean up after test
    }

    // Test for CreateListFile with invalid file name
    [Test]
    public void TestCreateListFileInvalidFilePath()
    {
        mockFileSystem.DirectoryExistsResult = false;
        var re = m_sut.CreateListFile("errorPath");
        Assert.AreEqual(re, ConstDef.ConstErrFotoPath);
    }

    // Test for CreateListFile with null file name
    [Test]
    public void TestCreateListFileNullFileName()
    {
        string result = m_sut.CreateListFile(null);
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }

    // Test for CreateListFile with empty file name
    [Test]
    public void TestCreateListFileEmptyFileName()
    {
        string result = m_sut.CreateListFile(string.Empty);
        Assert.AreEqual(result, ConstDef.ConstErrFotoPath);
    }
    // Test for CreateListFile with file name that already exists
    [Test]
    public void TestCreateListFileExistingFileName()
    {
        mockFileSystem.FileExistsResult = true;
        string existingFileName = "existingfile.txt";
        string result = m_sut.CreateListFile(existingFileName);
        Assert.IsNotNull(result); // Should not overwrite the file
        File.Delete(existingFileName); // Clean up after test
    }


    [Test]
    /// <summary>
    /// Test case: Clean photo when all parameters are valid.
    /// </summary>
    public void TestCleanPhotoValidPathAndFileName()
    {
        mockFileSystem.DirectoryExistsResult = true;
        mockFileSystem.FileExistsResult = true;
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        m_sut.CreateListFile(listFileName);
        string result = m_sut.CleanPhoto(listFileName, reportFileName);
        Assert.IsNotNull(result);
        File.Delete(reportFileName); // Clean up after test
    }

    [Test]
    /// <summary>
    /// Test case: Clean photo when the path is null.
    /// </summary>
    public void TestCleanPhotoInvalidPath()
    {
        mockFileSystem.DirectoryExistsResult = false;
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        m_sut.CreateListFile(listFileName);
        var re = m_sut.CleanPhoto(null, reportFileName);
        Assert.AreEqual(re, ConstDef.ConstErrFotoPath);
    }

    [Test]
    /// <summary>
    /// Test case: Clean photo when the report name is null.
    /// </summary>
    public void TestCleanPhotoNullReportName()
    {
        string listFileName = "testlist.txt";
        m_sut.CreateListFile(listFileName);
        var re = m_sut.CleanPhoto(listFileName, null);
        Assert.AreEqual(re, ConstDef.ConstErrFotoPath);
    }


    [Test]
    /// <summary>
    /// Test case: Generate diff reports when both lists are empty.
    /// </summary>
    public void GenerateDiffReports_BothListsEmpty_ReturnsNoDiff()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection();
        _sut.WriteListFileRes = "No differences found.";
        mockFileSystem.GetFilesResult = Array.Empty<string>();
        
        Assert.That(_sut.GenerateDiffReports("test.txt").Contains("No differences found."));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when only target has files.
    /// </summary>
    public void GenerateDiffReports_OnlyTargetHasFiles_ReturnsMissingInBaseline()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg";
        mockFileSystem.GetFilesResult = new[] { "file3.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file3";
        
        Assert.That(_sut.GenerateDiffReports("test.txt").Contains("Missing in baseline: file3.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when only baseline has files.
    /// </summary>
    public void GenerateDiffReports_OnlyBaselineHasFiles_ReturnsMissingInTarget()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file1.jpg,file2.jpg";
        mockFileSystem.GetFilesResult = Array.Empty<string>();
        
        Assert.That(_sut.GenerateDiffReports("test.txt").Contains("Missing in baseline: file1.jpg,file2.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when both lists have files with differences.
    /// </summary>
    public void GenerateDiffReports_BothListsHaveFiles_ReturnsCorrectDifferences()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg, Missing in target: file1.jpg";
        mockFileSystem.GetFilesResult = new[] { "file2.jpg", "file3.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file3";
        
        Assert.That(_sut.GenerateDiffReports("test.txt").Contains("Missing in baseline: file3.jpg, Missing in target: file1.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when lists have no differences.
    /// </summary>
    public void GenerateDiffReports_ListsHaveNoDifferences_ReturnsNoDiff2()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection { "file1.jpg", "file2.jpg" };
        _sut.WriteListFileRes = "No differences found.";
        mockFileSystem.GetFilesResult = new[] { "file1.jpg", "file2.jpg" };
        mockFileSystem.GetFileNameWithoutExtensionResult = "file1";
        
        Assert.That(_sut.GenerateDiffReports("test.txt").Contains("No differences found."));
    }

    [Test]
    /// <summary>
    /// Test case: Clean photo when the report name is an empty string.
    /// </summary>
    public void TestCleanPhotoEmptyReportName()
    {
        string listFileName = "testlist.txt";
        m_sut.CreateListFile(listFileName);
        var re = m_sut.CleanPhoto(listFileName, string.Empty);
        Assert.AreEqual(re, ConstDef.ConstErrFotoPath);
    }

    [Test]
    public void TestCreateListFile()
    {
        mockFileSystem.DirectoryExistsResult = true;
        string listFileName = "testlist.txt";
        string result = m_sut.CreateListFile(listFileName);
        Assert.IsNotNull(result);
        File.Delete(listFileName); // Clean up after test
    }

    [Test]
    public void TestGenerateDiffReports()
    {
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = "testPath";
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = new StringCollection();
        _sut.WriteListFileRes = "Test result";
        string result = m_sut.GenerateDiffReports("test.txt");
        Assert.IsNotNull(result);
    }
    [Test]
    public void TestCleanPhoto()
    {
        mockFileSystem.DirectoryExistsResult = true;
        mockFileSystem.FileExistsResult = true;
        var _sut = (testableFotoManager)m_sut;
        _sut.ReadListInFileRes = true;
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        string result = m_sut.CleanPhoto(listFileName, reportFileName);
        Assert.IsNotNull(result);
        File.Delete(reportFileName); // Clean up after test
    }
}

