using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasVogenConversion()
            .IsRequired();

        builder.Property(e => e.Content)
            .HasVogenConversion()
            .HasMaxLength(PostContent.MaxLength)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasVogenConversion()
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasIndex(e => e.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
