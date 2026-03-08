using FileUploader.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileUploader.Data.Config;

public class FileEntityConfig : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> b)
    {
        b.ToTable("Files");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();
        
        b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        b.Property(x => x.MimeType).IsRequired().HasMaxLength(128);
        b.Property(x => x.Size).IsRequired();
        b.Property(x => x.Path).IsRequired();

        b.HasOne(x => x.User)
            .WithMany(u => u.Files)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}