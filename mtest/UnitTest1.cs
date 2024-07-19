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

    [Test]
    public void should_get_err_message_when_foto_path_invalide()
    {
        var re = m_sut.CreateListFile("errorPath");
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

