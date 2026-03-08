using Microsoft.EntityFrameworkCore;

namespace FileUploader.Entities;

public class FileContext : DbContext
{
    public DbSet<FileEntity> Files { get; set; }
    
    public FileContext(DbContextOptions<FileContext> options) : base(options)
    {
    }
}