using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
using System;

namespace mtest;

public class MTestCrossPlatform
{
    private IFotoManger _originalManager;
    private testableFotoManager _testManager;
    private MockFileSystem _mockFileSystem;

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
        Program.Manager = _originalManager;
        _testManager = null;
        _mockFileSystem = null;
    }

    [Test]
    public void TestPathNormalization_WindowsStylePath()
    {
        // Arrange
        string windowsPath = @"C:\Users\Test\Photos\vacation.jpg";
        string expectedPath = Path.Combine("C:", "Users", "Test", "Photos", "vacation.jpg");

        // Act
        string normalizedPath = Program.NormalizePath(windowsPath);

        // Assert
        Assert.That(normalizedPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void TestPathNormalization_UnixStylePath()
    {
        // Arrange
        string unixPath = "/home/user/photos/vacation.jpg";
        string expectedPath = Path.Combine("home", "user", "photos", "vacation.jpg");

        // Act
        string normalizedPath = Program.NormalizePath(unixPath);

        // Assert
        Assert.That(normalizedPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void TestPathNormalization_MixedStylePath()
    {
        // Arrange
        string mixedPath = "C:/Users\\Test/Photos\\vacation.jpg";
        string expectedPath = Path.Combine("C:", "Users", "Test", "Photos", "vacation.jpg");

        // Act
        string normalizedPath = Program.NormalizePath(mixedPath);

        // Assert
        Assert.That(normalizedPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void TestFileSystem_CombinePaths()
    {
        // Arrange
        string[] paths = new[] { "base", "folder", "file.jpg" };
        string expectedPath = Path.Combine(paths);

        // Act
        string combinedPath = _mockFileSystem.Combine(paths);

        // Assert
        Assert.That(combinedPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void TestFileSystem_GetFullPath()
    {
        // Arrange
        string relativePath = "photos/vacation.jpg";
        string expectedPath = Path.GetFullPath(relativePath);

        // Act
        string fullPath = _mockFileSystem.GetFullPath(relativePath);

        // Assert
        Assert.That(fullPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void TestFileSystem_GetFileName()
    {
        // Arrange
        string path = Path.Combine("folder", "subfolder", "photo.jpg");
        string expectedFileName = Path.GetFileName(path);

        // Act
        string fileName = _mockFileSystem.GetFileName(path);

        // Assert
        Assert.That(fileName, Is.EqualTo(expectedFileName));
    }

    [Test]
    public void TestFileSystem_GetFileNameWithoutExtension()
    {
        // Arrange
        string path = Path.Combine("folder", "subfolder", "photo.jpg");
        string expectedName = Path.GetFileNameWithoutExtension(path);

        // Act
        string fileName = _mockFileSystem.GetFileNameWithoutExtension(path);

        // Assert
        Assert.That(fileName, Is.EqualTo(expectedName));
    }

    [Test]
    public void TestFileSystem_GetExtension()
    {
        // Arrange
        string path = Path.Combine("folder", "subfolder", "photo.jpg");
        string expectedExtension = Path.GetExtension(path);

        // Act
        string extension = _mockFileSystem.GetExtension(path);

        // Assert
        Assert.That(extension, Is.EqualTo(expectedExtension));
    }

    [Test]
    public void TestCreateListFile_CrossPlatformPath()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem.DirectoryExistsResult = true;
        _mockFileSystem.FileExistsResult = false;

        // Act
        string result = _testManager.CreateListFile(listFileName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void TestCleanPhoto_CrossPlatformPath()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        string reportFileName = Path.Combine("test", "folder", "report.txt");
        _mockFileSystem.FileExistsResult = true;
        _mockFileSystem.DirectoryExistsResult = true;
        _testManager.ReadListInFileRes = true;

        // Act
        string result = _testManager.CleanPhoto(listFileName, reportFileName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void TestGenerateDiffReports_CrossPlatformPath()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem.FileExistsResult = true;
        _testManager.ReadListInFileRes = true;
        _testManager.WriteListFileRes = "Diff report generated";

        // Act
        string result = _testManager.GenerateDiffReports(listFileName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void TestPathHandling_WithSpaces()
    {
        // Arrange
        string pathWithSpaces = Path.Combine("My Photos", "Vacation 2023", "beach photo.jpg");
        string expectedPath = Path.Combine("My Photos", "Vacation 2023", "beach photo.jpg");

        // Act
        string normalizedPath = Program.NormalizePath(pathWithSpaces);

        // Assert
        Assert.That(normalizedPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public void TestPathHandling_WithSpecialCharacters()
    {
        // Arrange
        string pathWithSpecialChars = Path.Combine("Photos", "Event (2023)", "photo-1.jpg");
        string expectedPath = Path.Combine("Photos", "Event (2023)", "photo-1.jpg");

        // Act
        string normalizedPath = Program.NormalizePath(pathWithSpecialChars);

        // Assert
        Assert.That(normalizedPath, Is.EqualTo(expectedPath));
    }
} 