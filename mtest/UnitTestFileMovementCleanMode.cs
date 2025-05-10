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
        string sourcePath = "source/photo1.jpg";
        string targetPath = "target/photo1.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveMultipleFiles()
    {
        // Arrange
        string sourcePath1 = "source/photo1.jpg";
        string sourcePath2 = "source/photo2.jpg";
        string targetPath1 = "target/photo1.jpg";
        string targetPath2 = "target/photo2.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath1);
        _testManager.AllPhotos.Add(sourcePath2);

        // Act
        string result1 = _testManager.MoveFile(sourcePath1, targetPath1);
        string result2 = _testManager.MoveFile(sourcePath2, targetPath2);

        // Assert
        Assert.That(result1, Is.EqualTo(ConstDef.ConstErrFotoPath));
        Assert.That(result2, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithExistingTarget()
    {
        // Arrange
        string sourcePath = "source/photo1.jpg";
        string targetPath = "target/photo1.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithInvalidTarget()
    {
        // Arrange
        string sourcePath = "source/photo1.jpg";
        string targetPath = "invalid/path/photo1.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = false;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithSubdirectories()
    {
        // Arrange
        string sourcePath = "source/subfolder/photo1.jpg";
        string targetPath = "target/subfolder/photo1.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithSpecialCharacters()
    {
        // Arrange
        string sourcePath = "source/photo with spaces.jpg";
        string targetPath = "target/photo with spaces.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithLongPath()
    {
        // Arrange
        string sourcePath = "source/" + new string('a', 260) + ".jpg";
        string targetPath = "target/" + new string('a', 260) + ".jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithDifferentExtensions()
    {
        // Arrange
        string sourcePath = "source/photo1.png";
        string targetPath = "target/photo1.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();
        _testManager.AllPhotos.Add(sourcePath);

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }

    [Test]
    public void TestMoveFileWithEmptyList()
    {
        // Arrange
        string sourcePath = "source/photo1.jpg";
        string targetPath = "target/photo1.jpg";
        _mockFileSystem!.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.OpenTextResult = new StreamReader(new MemoryStream());
        _testManager!.ReadListInFileRes = true;
        _testManager.AllPhotos = new StringCollection();

        // Act
        string result = _testManager.MoveFile(sourcePath, targetPath);

        // Assert
        Assert.That(result, Is.EqualTo(ConstDef.ConstErrFotoPath));
    }
} 