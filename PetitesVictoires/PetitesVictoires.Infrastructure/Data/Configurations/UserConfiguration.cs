using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Id)
            .HasVogenConversion()
            .IsRequired();

        builder.Property(e => e.EmailAddress)
            .HasVogenConversion()
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasVogenConversion()
            .HasMaxLength(UserName.MaxLength)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
