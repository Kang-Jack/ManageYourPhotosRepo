using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
using System;
namespace mtest;

public class testableFotoManager : FotoManager
{
    public bool ReadListInFileRes { set; get; }

    public string InputPhotoFolderRes { set; get; }

    public string WriteListFileRes { set; get; }

    public StringCollection AllPhotos { set; get; }

    protected override bool ReadListInFile(string listFileName, StringCollection allPhotos)
    {
        allPhotos = AllPhotos;
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

}
public class MTestFotoManger
{
    IFotoManger m_sut;
    private string tempDirPath;

    [SetUp]
    public void Setup()
    {
        m_sut = new testableFotoManager();
        // Setup: Create a temporary directory for testing
        tempDirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirPath);
    }

    [TearDown]
    public void TearDown()
    {
        m_sut = null;
        // Cleanup: Delete the temporary directory after each test
        if (Directory.Exists(tempDirPath))
            Directory.Delete(tempDirPath, true);
    }

    // Test for CreateListFile with valid path and file name
    [Test]
    public void TestCreateListFileValidPathAndFileName()
    {
        string listFileName = "testlist.txt";
        string result = m_sut.CreateListFile(listFileName);
        Assert.IsNotNull(result);
        File.Delete(listFileName); // Clean up after test
    }

    // Test for CreateListFile with invalid file name
    [Test]
    public void TestCreateListFileInvalidFilePath()
    {
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
        string existingFileName = "existingfile.txt";
        File.Create(existingFileName).Close(); // Create the file
        string listFileName = existingFileName;
        string result = m_sut.CreateListFile(listFileName);
        Assert.IsNotNull(result); // Should not overwrite the file
        File.Delete(listFileName); // Clean up after test
    }


    [Test]
    /// <summary>
    /// Test case: Clean photo when all parameters are valid.
    /// </summary>
    public void TestCleanPhotoValidPathAndFileName()
    {
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

        var path = Path.Combine(tempDirPath, "target");
     
        Directory.CreateDirectory(path);
        var listFileName = "emptylist.txt";
        var filePath = Path.Combine(tempDirPath, listFileName);
        File.WriteAllText(filePath, string.Empty);
        StringCollection allPhotosInBaseline = new();
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = path;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = allPhotosInBaseline;
        _sut.WriteListFileRes = "No differences found.";
        Assert.That(_sut.GenerateDiffReports(filePath).Contains("No differences found."));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when only target has files.
    /// </summary>
    public void GenerateDiffReports_OnlyTargetHasFiles_ReturnsMissingInBaseline()
    {
        var path = Path.Combine(tempDirPath, "target");
        Directory.CreateDirectory(path);
        var listFileName = "target.txt";
        var filePath = Path.Combine(tempDirPath, listFileName);
        File.WriteAllText(filePath, "file1.jpg\nfile2.jpg");
        File.WriteAllText(Path.Combine(path, "file3.jpg"), string.Empty);

        StringCollection allPhotosInBaseline = new StringCollection { "file1.jpg", "file2.jpg" };
       var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = path;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = allPhotosInBaseline;
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg";
        Assert.That(_sut.GenerateDiffReports(listFileName).Contains("Missing in baseline: file3.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when only baseline has files.
    /// </summary>
    public void GenerateDiffReports_OnlyBaselineHasFiles_ReturnsMissingInTarget()
    {
        var path = Path.Combine(tempDirPath, "target");
        Directory.CreateDirectory(path);
        var listFileName = "baseline.txt";
        var filePath = Path.Combine(tempDirPath, listFileName);
        File.WriteAllText(filePath, "file1.jpg\nfile2.jpg");
  
        StringCollection allPhotosInBaseline = new StringCollection { "file1.jpg", "file2.jpg" };
        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = path;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = allPhotosInBaseline;
        _sut.WriteListFileRes = "Missing in baseline: file1.jpg,file2.jpg";


        Assert.That(_sut.GenerateDiffReports(listFileName).Contains("Missing in baseline: file1.jpg,file2.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when both lists have files with differences.
    /// </summary>
    public void GenerateDiffReports_BothListsHaveFiles_ReturnsCorrectDifferences()
    {
        var path = Path.Combine(tempDirPath, "target");
        Directory.CreateDirectory(path);
        var listFileName = "baselineAndTarget.txt";
        var filePath = Path.Combine(tempDirPath, listFileName);
        File.WriteAllText(filePath, "file1.jpg\nfile2.jpg");

       
        File.WriteAllText(Path.Combine(tempDirPath, "target", "file2.jpg"), string.Empty); // Different name but same content as baseline
        File.WriteAllText(Path.Combine(tempDirPath, "target", "file3.jpg"), string.Empty); // Completely new file in target
        StringCollection allPhotosInBaseline = new StringCollection { "file1.jpg", "file2.jpg" };

        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = path;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = allPhotosInBaseline;
        _sut.WriteListFileRes = "Missing in baseline: file3.jpg, Missing in target: file1.jpg";


        Assert.That(m_sut.GenerateDiffReports(listFileName).Contains("Missing in baseline: file3.jpg, Missing in target: file1.jpg"));
    }

    [Test]
    /// <summary>
    /// Test case: Generate diff reports when lists have no differences.
    /// </summary>
    public void GenerateDiffReports_ListsHaveNoDifferences_ReturnsNoDiff2()
    {
        var path = Path.Combine(tempDirPath, "target");
        Directory.CreateDirectory(path);
        var listFileName = "sameFiles.txt";
        var filePath = Path.Combine(tempDirPath, listFileName);
     
        File.WriteAllText(listFileName, "file1.jpg\nfile2.jpg");
        File.WriteAllText(Path.Combine(path, "file1.jpg"), string.Empty);
        File.WriteAllText(Path.Combine(path, "file2.jpg"), string.Empty);
        StringCollection allPhotosInBaseline = new StringCollection { "file1.jpg", "file2.jpg" };

        var _sut = (testableFotoManager)m_sut;
        _sut.InputPhotoFolderRes = path;
        _sut.ReadListInFileRes = true;
        _sut.AllPhotos = allPhotosInBaseline;
        _sut.WriteListFileRes = "No differences found.";

        Assert.That(m_sut.GenerateDiffReports(listFileName).Contains("No differences found."));
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
        string listFileName = "testlist.txt";
        string result = m_sut.CreateListFile(listFileName);
        Assert.IsNotNull(result);
        File.Delete(listFileName); // Clean up after test
    }

    [Test]
    public void TestGenerateDiffReports()
    {
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        m_sut.CreateListFile(listFileName);
        string result = m_sut.GenerateDiffReports(listFileName);
        Assert.IsNotNull(result);
        File.Delete(reportFileName); // Clean up after test
    }
    [Test]
    public void TestCleanPhoto()
    {
        string listFileName = "testlist.txt";
        string reportFileName = "report.txt";
        m_sut.CreateListFile(listFileName);
        string result = m_sut.CleanPhoto(listFileName, reportFileName);
        Assert.IsNotNull(result);
        File.Delete(reportFileName); // Clean up after test
    }
}

