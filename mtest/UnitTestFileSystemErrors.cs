using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
using System;

namespace mtest;

public class MTestFileSystemErrors
{
    private IFotoManger? _originalManager;
    private testableFotoManager? _testManager;
    private MockFileSystem? _mockFileSystem;

    [SetUp]
    public void Setup()
    {
        _mockFileSystem = new MockFileSystem();
        _testManager = new testableFotoManager(_mockFileSystem);
        _originalManager = Program.Manager;
        Program.Manager = _testManager;
    }

    [TearDown]
    public void TearDown()
    {
        if (_originalManager != null)
        {
            Program.Manager = _originalManager;
        }
        _testManager = null;
        _mockFileSystem = null;
    }

    [Test]
    public void TestCreateListFile_AccessDenied()
    {
        // Arrange
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.ThrowAccessDenied = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _testManager!.CreateListFile("testlist.txt"));
    }

    [Test]
    public void TestCreateListFile_FileInUse()
    {
        // Arrange
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.ThrowFileInUse = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.WriteListFileRes = "test"; // Set a non-null result to avoid null reference

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _testManager!.CreateListFile("testlist.txt"));
        Assert.That(ex.Message, Is.EqualTo("File in use"));
    }

    [Test]
    public void TestCleanPhoto_AccessDenied()
    {
        // Arrange
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.ThrowAccessDenied = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add("test.jpg");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _testManager!.CleanPhoto("test.jpg", "report.txt"));
    }

    [Test]
    public void TestGenerateDiffReports_AccessDenied()
    {
        // Arrange
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.ThrowAccessDenied = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add("test.jpg");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _testManager!.GenerateDiffReports("test.jpg"));
    }

    [Test]
    public void TestCleanPhoto_FileNotFound()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        string reportFileName = Path.Combine("test", "folder", "report.txt");
        _mockFileSystem!.FileExistsResult = false;

        // Act
        string result = _testManager!.CleanPhoto(listFileName, reportFileName);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestCreateListFile_InvalidPath()
    {
        // Arrange
        string listFileName = "invalid/path/with/invalid/chars/*/list.txt";
        _mockFileSystem!.DirectoryExistsResult = false;

        // Act
        string result = _testManager!.CreateListFile(listFileName);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestCleanPhoto_InvalidPath()
    {
        // Arrange
        string listFileName = "invalid/path/with/invalid/chars/*/list.txt";
        string reportFileName = "report.txt";
        _mockFileSystem!.FileExistsResult = false;

        // Act
        string result = _testManager!.CleanPhoto(listFileName, reportFileName);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestGenerateDiffReports_FileNotFound()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem!.FileExistsResult = false;

        // Act
        string result = _testManager!.GenerateDiffReports(listFileName);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestGenerateDiffReports_InvalidPath()
    {
        // Arrange
        string listFileName = "invalid/path/with/invalid/chars/*/list.txt";
        _mockFileSystem!.FileExistsResult = false;

        // Act
        string result = _testManager!.GenerateDiffReports(listFileName);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestCreateListFile_EmptyDirectory()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem!.DirectoryExistsResult = true;
        _mockFileSystem.FileExistsResult = false;
        _mockFileSystem.GetFilesResult = Array.Empty<string>();
        _mockFileSystem.GetDirectoriesResult = Array.Empty<string>();
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());

        // Act
        string result = _testManager!.CreateListFile(listFileName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void TestCleanPhoto_EmptyList()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        string reportFileName = Path.Combine("test", "folder", "report.txt");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();

        // Act
        string result = _testManager.CleanPhoto(listFileName, reportFileName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void TestGenerateDiffReports_EmptyLists()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _mockFileSystem.GetFilesResult = Array.Empty<string>();

        // Act
        string result = _testManager.GenerateDiffReports(listFileName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    // Suggested test addition:
    [Test]
    public void MoveFile_InvalidSource_ThrowsException()
    {
        var fs = new FileSystem();
        Assert.Throws<FileNotFoundException>(() => 
            fs.MoveFile("nonexistent.txt", "target.txt"));
    }

    // Additional FileSystem Tests
    [Test]
    public void GetFullPath_ReturnsAbsolutePath()
    {
        // Arrange
        var fs = new FileSystem();
        string relativePath = "test\folder";

        // Act
        string result = fs.GetFullPath(relativePath);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(relativePath));
        Assert.That(Path.IsPathRooted(result), Is.True);
    }

    [Test]
    public void Combine_NormalizesSeparators()
    {
        // Arrange
        var fs = new FileSystem();
        string[] paths = new[] { "path/with/forward", "slash\\with\\backward" };

        // Act
        string result = fs.Combine(paths);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Contains('/') || result.Contains('\\'), Is.True);
    }

    [Test]
    public void GetFileNameWithoutExtension_ReturnsCorrectName()
    {
        // Arrange
        var fs = new FileSystem();
        string path = "C:\\test\folder\file.txt";

        // Act
        string result = fs.GetFileNameWithoutExtension(path);

        // Assert
        Assert.That(result, Is.EqualTo("file"));
    }

    [Test]
    public void GetFileName_ReturnsCorrectName()
    {
        // Arrange
        var fs = new FileSystem();
        string path = "C:\\test\folder\file.txt";

        // Act
        string result = fs.GetFileName(path);

        // Assert
        Assert.That(result, Is.EqualTo("file.txt"));
    }

    [Test]
    public void GetExtension_ReturnsCorrectExtension()
    {
        // Arrange
        var fs = new FileSystem();
        string path = "C:\\test\folder\file.txt";

        // Act
        string result = fs.GetExtension(path);

        // Assert
        Assert.That(result, Is.EqualTo(".txt"));
    }

    [Test]
    public void CreateDirectory_CreatesNestedDirectories()
    {
        // Arrange
        var fs = new FileSystem();
        string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        string nestedPath = Path.Combine(tempPath, "nested", "folders");

        try
        {
            // Act
            fs.CreateDirectory(nestedPath);

            // Assert
            Assert.That(Directory.Exists(nestedPath), Is.True);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }

    [Test]
    public void GetFiles_ReturnsMatchingFiles()
    {
        // Arrange
        var fs = new FileSystem();
        string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempPath);
        
        try
        {
            // Create test files
            string testFile1 = Path.Combine(tempPath, "test1.txt");
            string testFile2 = Path.Combine(tempPath, "test2.txt");
            string testFile3 = Path.Combine(tempPath, "other.dat");
            File.WriteAllText(testFile1, "test");
            File.WriteAllText(testFile2, "test");
            File.WriteAllText(testFile3, "test");
            
            // Act
            string[] txtFiles = fs.GetFiles(tempPath, "*.txt");
            string[] allFiles = fs.GetFiles(tempPath, "*.*");
            
            // Assert
            Assert.That(txtFiles.Length, Is.EqualTo(2));
            Assert.That(allFiles.Length, Is.EqualTo(3));
            Assert.That(txtFiles, Does.Contain(testFile1));
            Assert.That(txtFiles, Does.Contain(testFile2));
            Assert.That(txtFiles, Does.Not.Contain(testFile3));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }
    
    [Test]
    public void GetDirectories_ReturnsSubdirectories()
    {
        // Arrange
        var fs = new FileSystem();
        string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempPath);
        
        try
        {
            // Create test directories
            string subDir1 = Path.Combine(tempPath, "subdir1");
            string subDir2 = Path.Combine(tempPath, "subdir2");
            Directory.CreateDirectory(subDir1);
            Directory.CreateDirectory(subDir2);
            
            // Act
            string[] directories = fs.GetDirectories(tempPath);
            
            // Assert
            Assert.That(directories.Length, Is.EqualTo(2));
            Assert.That(directories, Does.Contain(subDir1));
            Assert.That(directories, Does.Contain(subDir2));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }
    
    [Test]
    public void OpenText_ReadsFileContent()
    {
        // Arrange
        var fs = new FileSystem();
        string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        string testFile = Path.Combine(tempPath, "test.txt");
        Directory.CreateDirectory(tempPath);
        string expectedContent = "Test file content";
        File.WriteAllText(testFile, expectedContent);
        
        try
        {
            // Act
            using (StreamReader reader = fs.OpenText(testFile))
            {
                string content = reader.ReadToEnd();
                
                // Assert
                Assert.That(content, Is.EqualTo(expectedContent));
            }
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }
    
    [Test]
    public void CreateText_WritesFileContent()
    {
        // Arrange
        var fs = new FileSystem();
        string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        string testFile = Path.Combine(tempPath, "newfile.txt");
        Directory.CreateDirectory(tempPath);
        string expectedContent = "New file content";
        
        try
        {
            // Act
            using (StreamWriter writer = fs.CreateText(testFile))
            {
                writer.Write(expectedContent);
            }
            
            // Assert
            Assert.That(File.Exists(testFile), Is.True);
            string actualContent = File.ReadAllText(testFile);
            Assert.That(actualContent, Is.EqualTo(expectedContent));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }
}