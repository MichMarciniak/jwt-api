using FileUploader.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileUploader.Data.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
       b.ToTable("Users");
       
       b.HasKey(x => x.Id);
       b.Property(x => x.Id).ValueGeneratedOnAdd();
       
       b.Property(x => x.Name).IsRequired().HasMaxLength(100);
       b.Property(x => x.Password).IsRequired();

       b.HasMany(x => x.Files)
           .WithOne(x => x.User)
           .HasForeignKey(x => x.UserId);
    }
}