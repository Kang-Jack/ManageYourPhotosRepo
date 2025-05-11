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
}