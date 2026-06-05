using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetitesVictoires.Core.PostAggregate;

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

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
