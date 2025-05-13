using System.Collections.Specialized;
using foto_list;
using NUnit.Framework;

namespace mtest;

[TestFixture]
public class UnitTestDiffReportDTO
{
    private DiffReportDTO _diffReport;

    [SetUp]
    public void Setup()
    {
        _diffReport = new DiffReportDTO
        {
            DelPhotos = new StringCollection(),
            SubFolderNames = new StringCollection()
        };
    }

    [Test]
    public void TestPhotoCountInitialization()
    {
        // Arrange & Act
        var report = new DiffReportDTO();

        // Assert
        Assert.That(report.TotalBaselinePhotoNum, Is.Null);
        Assert.That(report.TotalTargetFolderPhotoNum, Is.Null);
    }

    [Test]
    public void TestPhotoCountCalculation()
    {
        // Arrange
        _diffReport.TotalBaselinePhotoNum = "100";
        _diffReport.TotalTargetFolderPhotoNum = "80";

        // Assert
        Assert.That(_diffReport.TotalBaselinePhotoNum, Is.EqualTo("100"));
        Assert.That(_diffReport.TotalTargetFolderPhotoNum, Is.EqualTo("80"));
    }

    [Test]
    public void TestDeletedPhotosCollection()
    {
        // Arrange
        string photo1 = "photo1.jpg";
        string photo2 = "photo2.jpg";

        // Act
        _diffReport.DelPhotos.Add(photo1);
        _diffReport.DelPhotos.Add(photo2);

        // Assert
        Assert.That(_diffReport.DelPhotos.Count, Is.EqualTo(2));
        Assert.That(_diffReport.DelPhotos, Does.Contain(photo1));
        Assert.That(_diffReport.DelPhotos, Does.Contain(photo2));
    }

    [Test]
    public void TestSubFolderCollection()
    {
        // Arrange
        string folder1 = "folder1";
        string folder2 = "folder2/subfolder";

        // Act
        _diffReport.SubFolderNames.Add(folder1);
        _diffReport.SubFolderNames.Add(folder2);

        // Assert
        Assert.That(_diffReport.SubFolderNames.Count, Is.EqualTo(2));
        Assert.That(_diffReport.SubFolderNames, Does.Contain(folder1));
        Assert.That(_diffReport.SubFolderNames, Does.Contain(folder2));
    }

    [Test]
    public void TestEmptyCollections()
    {
        // Assert
        Assert.That(_diffReport.DelPhotos.Count, Is.EqualTo(0));
        Assert.That(_diffReport.SubFolderNames.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestPhotoCountComparison()
    {
        // Arrange
        _diffReport.TotalBaselinePhotoNum = "22";
        _diffReport.TotalTargetFolderPhotoNum = "20";
        _diffReport.DelPhotos.Add("deleted1.jpg");
        _diffReport.DelPhotos.Add("deleted2.jpg");

        // Assert
        Assert.That(int.Parse(_diffReport.TotalBaselinePhotoNum) - int.Parse(_diffReport.TotalTargetFolderPhotoNum), 
                    Is.EqualTo(_diffReport.DelPhotos.Count));
    }
}