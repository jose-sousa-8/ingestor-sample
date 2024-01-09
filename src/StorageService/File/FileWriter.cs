namespace StorageService.File
{
    using File = System.IO.File;
    
    public class FileWriter : IFileWriter
    {
        public void EnsureFileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
        }
        
        public void AppendLine(string filePath, string content)
        {
            // assumes a single thread writing 
            // else should manage file access with locks
            using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            using var writer = new StreamWriter(fs);
            writer.WriteLine(content);
        }
    }
}