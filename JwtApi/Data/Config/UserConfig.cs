using JwtApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JwtApi.Data.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
       b.ToTable("Users");
       
       b.HasKey(x => x.Id);
       b.HasIndex(x => x.Id).IsUnique();
       b.Property(x => x.Id).ValueGeneratedOnAdd();
       
       b.HasIndex(x => x.Name).IsUnique();
       b.Property(x => x.Name).IsRequired().HasMaxLength(100);
       b.Property(x => x.Password).IsRequired();
       b.Property(x => x.TokenVersion).IsRequired().HasDefaultValue(1);

    }
}