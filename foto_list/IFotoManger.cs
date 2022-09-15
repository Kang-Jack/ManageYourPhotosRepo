namespace foto_list
{
    public interface IFotoManger
    {
        string CreateListFile(string listFileName);
        string GenerateDiffReport(string listFileName, string reportFileName);
        string CleanPhoto(string listFileName, string reportFileName);
    }
}

