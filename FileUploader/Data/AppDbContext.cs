using FileUploader.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileUploader.Data;

public class AppDbContext : DbContext
{
    public DbSet<FileEntity> Files { get; set; }
    public DbSet<User> Users { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}