using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasVogenConversion()
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasVogenConversion()
            .IsRequired();

        builder.Property(e => e.PostId)
            .HasVogenConversion()
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // A user cannot like the same post twice
        builder.HasIndex(e => new { e.UserId, e.PostId }).IsUnique();
        builder.HasIndex(e => e.PostId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
