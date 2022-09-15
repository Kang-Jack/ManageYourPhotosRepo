using System.Collections.Specialized;
using foto_list;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
namespace mtest
{
    public class testableFotoManager : FotoManager
    {
        public bool ReadListInFileRes{ set; get; }

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


        [Test]
        public void should_get_err_message_when_foto_path_invalide()
        {
            var re = m_sut.CreateListFile("errorPath");
            Assert.AreEqual(re, ConstDef.ConstErrFotoPath);
        }
    }
}
