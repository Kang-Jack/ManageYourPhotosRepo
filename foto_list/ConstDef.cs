using System;
using System.Collections.Specialized;
using System.IO;

namespace foto_list
{
    public static class ConstDef
    {
        public const string ConstParamPath = "path";
        public const string ConstParamCompare = "compare";
        public const string ConstParamClean = "clean";
        public const string ConstParamHelp = "help";
        public const string ConstFotoPath = "photo folder path";
        public const string ConstErrFotoPath = "the input path need double check !";
        public const string ConstErrFotolistFile = "photo list file need double check !";
        public const string ConstTempRemoveFolderName = "removed";
        public const string ConstErrWriteFile = "Faild to write list file: ";
        public const string ConstMesgReturnList = "The photo list is built in: ";
        public const string ConstlistFileName = "foto_list.txt";
        public const string ConstRemovedFileName = "removed_foto_list.txt";
        public const string ConstBaselineDiffFileName = "file_missing_baseline.txt";
        public const string ConstTargetDiffFileName = "file_missing_target.txt";
        public const string ConstErrInvalidparameter = "Invalid parameter, switch to help mode should be: --help";
        public const string ConstHelpList = "Use no parameter, switch to create list mode, to create photo list in baseline folder (sub folder include)";
        public const string ConstHelpClean = "Use parameter --path, switch to clean mode, to clean the target folder based on foto list, should be: --path \"foto list full path\"";
        public const string ConstHelpClean2 = "Use parameter --clean, switch to clean mode, to clean the target folder based on foto list, should be: --clean \"foto list full path\"";
        public const string ConstHelpCompare = "Use parameter --compare, switch to compare mode, to generate compare report for both baseline and target folder, should be: --compare \"foto list full path\"";
    }
    public class DiffReportDTO
    {
        public string TotalBaselinePhotoNum { get; set; }
        public string TotalTargetFolderPhotoNum { get; set; }
        public StringCollection DelPhotos { get; set; }
        public StringCollection SubFolderNames { get; set; }
    }
}

