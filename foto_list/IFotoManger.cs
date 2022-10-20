namespace foto_list
{
    public interface IFotoManger
    {
        string CreateListFile(string listFileName);
        string GenerateDiffReports(string listFileName);
        string CleanPhoto(string listFileName, string reportFileName);
    }
}

