using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Identity;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class UserSeeder
{
    // Shared password for every seeded account, handy for signing in as anyone during development.
    private const string SeedPassword = "Password123!";

    // Usernames also become the Identity username, so they must stay within Identity's default
    // allowed charset (ASCII letters/digits and - . _ @ +): no spaces, no accents.
    private static readonly (UserName Name, Email Email)[] SeedUsers =
    [
        (UserName.From("camille.laurent"), Email.From("camille.laurent@mail.com")),
        (UserName.From("lucas.moreau"), Email.From("lucas.moreau@mail.com")),
        (UserName.From("emma.dubois"), Email.From("emma.dubois@mail.com")),
        (UserName.From("thomas.bernard"), Email.From("thomas.bernard@mail.com")),
        (UserName.From("lea.petit"), Email.From("lea.petit@mail.com")),
        (UserName.From("hugo.girard"), Email.From("hugo.girard@mail.com"))
    ];

    public static async Task<List<UserId>> SeedAsync(PetitesVictoiresDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        if (await dbContext.Users.AnyAsync()) return []; // DB has been seeded

        return await PopulateDataAsync(dbContext, userManager);
    }

    private static async Task<List<UserId>> PopulateDataAsync(PetitesVictoiresDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        var ids = new List<UserId>();

        foreach (var (name, email) in SeedUsers)
        {
            // Let Identity create the AspNetUsers row (password hash, security stamp, normalized
            // fields), then mirror the generated id into the domain "Users" table.
            var applicationUser = new ApplicationUser { UserName = name.Value, Email = email.Value };
            var result = await userManager.CreateAsync(applicationUser, SeedPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to seed user '{name.Value}': {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO \"Users\" (\"Id\", \"EmailAddress\", \"Name\", \"CreatedAt\") VALUES ({applicationUser.Id}, {email.Value}, {name.Value}, {DateTime.UtcNow})");

            ids.Add(UserId.From(applicationUser.Id));
        }

        return ids;
    }
}
