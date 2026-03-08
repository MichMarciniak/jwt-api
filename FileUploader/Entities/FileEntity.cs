namespace FileUploader.Entities;

public class FileEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public DateTime LastModified { get; set; }
    public int Size { get; set; }
    public string MimeType { get; set; }
    
     
}