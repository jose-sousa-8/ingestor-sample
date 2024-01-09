namespace StorageService.File
{
    public interface IFileWriter
    {
        void EnsureFileExists(string filePath);
        
        void AppendLine(string filePath, string content);
    }
}