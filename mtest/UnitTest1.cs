using System.Collections.Specialized;
using foto_list;
using System.IO;
using NUnit.Framework;
namespace mtest;

public class testableFotoManager : FotoManager
{
    public bool ReadListInFileRes { set; get; }

    public string InputPhotoFolderRes { set; get; }

    public string WriteListFileRes { set; get; }

    protected override bool ReadListInFile(string listFileName, StringCollection allPhotos)
    {
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

    [SetUp]
    public void Setup()
    {
        m_sut = new testableFotoManager();
    }

    [TearDown]
    public void TearDown()
    {
        m_sut = null;
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

