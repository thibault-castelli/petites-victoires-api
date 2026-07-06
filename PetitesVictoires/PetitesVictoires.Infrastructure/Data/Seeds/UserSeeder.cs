using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class UserSeeder
{
    public static readonly UserId UserId1 = UserId.From(9998);
    public static readonly UserId UserId2 = UserId.From(9999);

    private static readonly Email Email1 = Email.From("example@mail.com");
    private static readonly Email Email2 = Email.From("johndoe@mail.com");

    private static readonly UserName Name1 = UserName.From("example");
    private static readonly UserName Name2 = UserName.From("johndoe");

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Users\" (\"Id\", \"EmailAddress\", \"Name\", \"CreatedAt\") VALUES ({UserId1.Value}, {Email1.Value}, {Name1.Value}, {DateTime.UtcNow})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Users\" (\"Id\", \"EmailAddress\", \"Name\", \"CreatedAt\") VALUES ({UserId2.Value}, {Email2.Value}, {Name2.Value}, {DateTime.UtcNow})");
    }
}
