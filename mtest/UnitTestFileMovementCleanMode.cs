using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
using System;
using System.Linq;

namespace mtest;

public class MTestFileMovementCleanMode
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
    public void TestMoveFileToNewDirectory()
    {
        // Arrange
        string sourcePath = Path.Combine("source", "photo1.jpg");
        string targetPath = Path.Combine("target", "photo1.jpg");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveMultipleFiles()
    {
        // Arrange
        var sourceFiles = new[]
        {
            Path.Combine("source", "photo1.jpg"),
            Path.Combine("source", "photo2.jpg"),
            Path.Combine("source", "photo3.jpg")
        };
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        foreach (var file in sourceFiles)
        {
            _testManager.AllPhotos.Add(file);
        }
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourceFiles[0], "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithExistingTarget()
    {
        // Arrange
        string sourcePath = Path.Combine("source", "photo1.jpg");
        string targetPath = Path.Combine("target", "photo1.jpg");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithInvalidSource()
    {
        // Arrange
        string sourcePath = Path.Combine("source", "nonexistent.jpg");
        _mockFileSystem!.FileExistsResult = false;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithInvalidTarget()
    {
        // Arrange
        string sourcePath = Path.Combine("source", "photo1.jpg");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = false; // Target directory doesn't exist
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithSubdirectories()
    {
        // Arrange
        string sourcePath = Path.Combine("source", "subdir", "photo1.jpg");
        string targetPath = Path.Combine("target", "subdir", "photo1.jpg");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithSpecialCharacters()
    {
        // Arrange
        string sourcePath = Path.Combine("source", "photo with spaces & special chars.jpg");
        string targetPath = Path.Combine("target", "photo with spaces & special chars.jpg");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithLongPath()
    {
        // Arrange
        string longPath = string.Join(Path.DirectorySeparatorChar.ToString(), 
            Enumerable.Repeat("subdir", 10).Concat(new[] { "photo1.jpg" }));
        string sourcePath = Path.Combine("source", longPath);
        string targetPath = Path.Combine("target", longPath);
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithDifferentExtensions()
    {
        // Arrange
        var sourceFiles = new[]
        {
            Path.Combine("source", "photo1.jpg"),
            Path.Combine("source", "photo2.png"),
            Path.Combine("source", "photo3.gif")
        };
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        foreach (var file in sourceFiles)
        {
            _testManager.AllPhotos.Add(file);
        }
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourceFiles[0], "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithEmptyList()
    {
        // Arrange
        string sourcePath = Path.Combine("source", "photo1.jpg");
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection(); // Empty list
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream()); // Simulate empty file

        // Act
        string result = _testManager!.CleanPhoto(sourcePath, "report.txt");

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }
} 