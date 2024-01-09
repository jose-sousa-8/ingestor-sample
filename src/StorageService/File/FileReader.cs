namespace StorageService.File
{
    using File = System.IO.File;
    
    public class FileReader : IFileReader
    {
        public string? ReadFile(string filePath)
        {
            return !File.Exists(filePath) ? null : File.ReadAllText(filePath);
        }
    }
}