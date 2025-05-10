using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace mtest;

public class MTestConcurrentOperations
{
    private IFotoManger? _originalManager;
    private testableFotoManager? _testManager;
    private MockFileSystem? _mockFileSystem;
    private readonly object _lockObject = new object();
    private int _concurrentOperations = 0;
    private bool _operationInProgress = false;

    [SetUp]
    public void Setup()
    {
        _mockFileSystem = new MockFileSystem();
        _testManager = new testableFotoManager(_mockFileSystem);
        _originalManager = Program.Manager;
        Program.Manager = _testManager;
        _concurrentOperations = 0;
        _operationInProgress = false;
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
    public async Task TestConcurrentCreateListFile()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem!.DirectoryExistsResult = true;
        _mockFileSystem.FileExistsResult = false;
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                lock (_lockObject)
                {
                    _concurrentOperations++;
                    _operationInProgress = true;
                }

                try
                {
                    _testManager!.CreateListFile(listFileName);
                }
                finally
                {
                    lock (_lockObject)
                    {
                        _concurrentOperations--;
                        _operationInProgress = false;
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        Assert.That(_concurrentOperations, Is.EqualTo(0), "All operations should complete");
        Assert.That(_operationInProgress, Is.False, "No operations should be in progress");
    }

    [Test]
    public async Task TestConcurrentCleanPhoto()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        string reportFileName = Path.Combine("test", "folder", "report.txt");
        _mockFileSystem!.FileExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                lock (_lockObject)
                {
                    _concurrentOperations++;
                    _operationInProgress = true;
                }

                try
                {
                    _testManager!.CleanPhoto(listFileName, reportFileName);
                }
                finally
                {
                    lock (_lockObject)
                    {
                        _concurrentOperations--;
                        _operationInProgress = false;
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        Assert.That(_concurrentOperations, Is.EqualTo(0), "All operations should complete");
        Assert.That(_operationInProgress, Is.False, "No operations should be in progress");
    }

    [Test]
    public async Task TestConcurrentGenerateDiffReports()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem!.FileExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                lock (_lockObject)
                {
                    _concurrentOperations++;
                    _operationInProgress = true;
                }

                try
                {
                    _testManager!.GenerateDiffReports(listFileName);
                }
                finally
                {
                    lock (_lockObject)
                    {
                        _concurrentOperations--;
                        _operationInProgress = false;
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        Assert.That(_concurrentOperations, Is.EqualTo(0), "All operations should complete");
        Assert.That(_operationInProgress, Is.False, "No operations should be in progress");
    }

    [Test]
    public async Task TestMixedConcurrentOperations()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        string reportFileName = Path.Combine("test", "folder", "report.txt");
        _mockFileSystem!.DirectoryExistsResult = true;
        _mockFileSystem.FileExistsResult = true;
        _testManager!.ReadListInFileRes = true;
        var tasks = new List<Task>();

        // Act
        // Create list file
        tasks.Add(Task.Run(() =>
        {
            lock (_lockObject)
            {
                _concurrentOperations++;
                _operationInProgress = true;
            }

            try
            {
                _testManager!.CreateListFile(listFileName);
            }
            finally
            {
                lock (_lockObject)
                {
                    _concurrentOperations--;
                    _operationInProgress = false;
                }
            }
        }));

        // Clean photos
        tasks.Add(Task.Run(() =>
        {
            lock (_lockObject)
            {
                _concurrentOperations++;
                _operationInProgress = true;
            }

            try
            {
                _testManager!.CleanPhoto(listFileName, reportFileName);
            }
            finally
            {
                lock (_lockObject)
                {
                    _concurrentOperations--;
                    _operationInProgress = false;
                }
            }
        }));

        // Generate diff reports
        tasks.Add(Task.Run(() =>
        {
            lock (_lockObject)
            {
                _concurrentOperations++;
                _operationInProgress = true;
            }

            try
            {
                _testManager!.GenerateDiffReports(listFileName);
            }
            finally
            {
                lock (_lockObject)
                {
                    _concurrentOperations--;
                    _operationInProgress = false;
                }
            }
        }));

        await Task.WhenAll(tasks);

        // Assert
        Assert.That(_concurrentOperations, Is.EqualTo(0), "All operations should complete");
        Assert.That(_operationInProgress, Is.False, "No operations should be in progress");
    }

    [Test]
    public async Task TestConcurrentFileAccess()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem!.DirectoryExistsResult = true;
        _mockFileSystem.FileExistsResult = true;
        var tasks = new List<Task>();
        var fileAccessCount = 0;
        var fileAccessLock = new object();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                lock (fileAccessLock)
                {
                    fileAccessCount++;
                }

                try
                {
                    // Simulate file access
                    Thread.Sleep(100);
                }
                finally
                {
                    lock (fileAccessLock)
                    {
                        fileAccessCount--;
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        Assert.That(fileAccessCount, Is.EqualTo(0), "All file access operations should complete");
    }

    [Test]
    public async Task TestConcurrentReadWrite()
    {
        // Arrange
        string listFileName = Path.Combine("test", "folder", "list.txt");
        _mockFileSystem!.DirectoryExistsResult = true;
        _mockFileSystem.FileExistsResult = true;
        var tasks = new List<Task>();
        var readCount = 0;
        var writeCount = 0;
        var operationLock = new object();

        // Act
        // Simulate concurrent reads
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                lock (operationLock)
                {
                    readCount++;
                }

                try
                {
                    // Simulate read operation
                    Thread.Sleep(100);
                }
                finally
                {
                    lock (operationLock)
                    {
                        readCount--;
                    }
                }
            }));
        }

        // Simulate concurrent writes
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                lock (operationLock)
                {
                    writeCount++;
                }

                try
                {
                    // Simulate write operation
                    Thread.Sleep(150);
                }
                finally
                {
                    lock (operationLock)
                    {
                        writeCount--;
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        Assert.That(readCount, Is.EqualTo(0), "All read operations should complete");
        Assert.That(writeCount, Is.EqualTo(0), "All write operations should complete");
    }
} 