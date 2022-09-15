using System;
using System.Collections.Specialized;
using System.IO;

namespace foto_list
{
    public static class ConstDef
    {
        public const string ConstParamPath = "-path";
        public const string ConstParamCompare = "-compare";

        public const string ConstFotoPath = "photo folder path";
        public const string ConstErrFotoPath = "photo path need double check !";
        public const string ConstErrFotolistFile = "photo list file need double check !";
        public const string ConstTempRemoveFolderName = "removed";
        public const string ConstErrWriteFile = "Faild to write list file: ";
        public const string ConstMesgReturnList = "The photo list is built in: ";
        public const string ConstlistFileName = "foto_list.txt";
        public const string ConstCleanFileName = "clean_foto_list.txt";
        public const string ConstErrInvalidparameter = "Invalid parameter, switch to clean mode should be: -path \"full path\"";
        
    }
    public class DiffReportDTO
    {
        public string TotalBaselinePhotoNum { get; set; }
        public string TotalTargetFolderPhotoNum { get; set; }
        public StringCollection DelPhotos { get; set; }
        public StringCollection SubFolderNames { get; set; }
    }
}

