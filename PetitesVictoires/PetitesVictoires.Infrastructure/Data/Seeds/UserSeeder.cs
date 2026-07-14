using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Identity;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class UserSeeder
{
    private static readonly Email Email1 = Email.From("example@mail.com");
    private static readonly Email Email2 = Email.From("johndoe@mail.com");

    private static readonly UserName Name1 = UserName.From("example");
    private static readonly UserName Name2 = UserName.From("johndoe");

    public static async Task<List<UserId>> SeedAsync(PetitesVictoiresDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        if (await dbContext.Users.AnyAsync()) return []; // DB has been seeded

        return await PopulateDataAsync(dbContext, userManager);
    }

    private static async Task<List<UserId>> PopulateDataAsync(PetitesVictoiresDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        var seedUsers = new[]
        {
            (Name1, Email1),
            (Name2, Email2)
        };
        var ids = new List<UserId>();

        foreach (var (name, email) in seedUsers)
        {
            var applicationUser = new ApplicationUser { UserName = name.Value, Email = email.Value };
            var result = await userManager.CreateAsync(applicationUser, "Password123!");
            if (!result.Succeeded) throw new InvalidOperationException();

            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO \"Users\" (\"Id\", \"EmailAddress\", \"Name\", \"CreatedAt\") VALUES ({applicationUser.Id}, {email.Value}, {name.Value}, {DateTime.UtcNow})");

            ids.Add(UserId.From(applicationUser.Id));
        }

        return ids;
    }
}
