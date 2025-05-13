using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
using System;

namespace mtest;

public class UnitTestFotoManagerOperations
{
    private IFotoManger? _originalManager;
    private TestableManager? _testManager;
    private MockFileSystem? _mockFileSystem;

    private class TestableManager : testableFotoManager
    {
        public TestableManager(IFileSystem fileSystem) : base(fileSystem) { }

        public new bool ReadListInFile(string listFileName, StringCollection allPhotos)
        {
            return base.ReadListInFile(listFileName, allPhotos);
        }

        public new string WriteListFile(string fileName, StringCollection allFiles)
        {
            return base.WriteListFile(fileName, allFiles);
        }
    }

    [SetUp]
    public void Setup()
    {
        _mockFileSystem = new MockFileSystem();
        _testManager = new TestableManager(_mockFileSystem);
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
    public void TestReadListInFile_Success()
    {
        // Arrange
        string testFile = "test.txt";
        var photoList = new StringCollection();
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection { "photo1.jpg", "photo2.jpg" };

        // Act
        bool result = _testManager!.ReadListInFile(testFile, photoList);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(photoList.Count, Is.EqualTo(2));
        Assert.That(photoList.Contains("photo1.jpg"));
        Assert.That(photoList.Contains("photo2.jpg"));
    }

    [Test]
    public void TestReadListInFile_FileNotFound()
    {
        // Arrange
        string testFile = "nonexistent.txt";
        _mockFileSystem!.OpenTextThrowException = true;
        var photoList = new StringCollection();

        // Act
        bool result = _testManager!.ReadListInFile(testFile, photoList);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(photoList.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestWriteListFile_Success()
    {
        // Arrange
        string testFile = "output.txt";
        var fileList = new StringCollection { "photo1.jpg", "photo2.jpg" };
        _testManager!.WriteListFileRes = ConstDef.ConstMesgReturnList + testFile;

        // Act
        string result = _testManager!.WriteListFile(testFile, fileList);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstMesgReturnList + testFile));
    }

    [Test]
    public void TestWriteListFile_Error()
    {
        // Arrange
        string testFile = "error.txt";
        var fileList = new StringCollection { "photo1.jpg" };
        _testManager!.WriteListFileRes = ConstDef.ConstErrWriteFile + testFile;

        // Act
        string result = _testManager!.WriteListFile(testFile, fileList);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrWriteFile + testFile));
    }

    [Test]
    public void TestCleanPhoto_InvalidPath()
    {
        // Arrange
        _mockFileSystem!.DirectoryExistsResult = false;
        _testManager!.InputPhotoFolderRes = string.Empty;

        // Act
        string result = _testManager.CleanPhoto("list.txt", "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestCleanPhoto_InvalidListFile()
    {
        // Arrange
        _mockFileSystem!.DirectoryExistsResult = true;
        _testManager!.InputPhotoFolderRes = "C:\\photos";
        _testManager.ReadListInFileRes = false;

        // Act
        string result = _testManager.CleanPhoto("invalid.txt", "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestCleanPhoto_Success()
    {
        // Arrange
        string testPath = "C:\\photos";
        string[] testFiles = { "photo1.jpg", "photo2.jpg", "photo3.jpg" };
        _mockFileSystem!.DirectoryExistsResult = true;
        _mockFileSystem.GetFilesResult = testFiles;
        _testManager!.InputPhotoFolderRes = testPath;
        _testManager.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection { "photo1", "photo2" };
        _testManager.WriteListFileRes = ConstDef.ConstMesgReturnList + "report.txt";

        // Act
        string result = _testManager.CleanPhoto("list.txt", "report.txt");

        // Assert
        Assert.That(result, Contains.Substring(ConstDef.ConstMesgReturnList));
    }
}