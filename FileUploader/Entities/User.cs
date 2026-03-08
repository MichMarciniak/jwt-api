namespace FileUploader.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }

    public ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
}