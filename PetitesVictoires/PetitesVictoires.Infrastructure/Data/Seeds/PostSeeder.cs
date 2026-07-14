using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class PostSeeder
{
    // AuthorIndex maps into the seeded users list (see UserSeeder.SeedUsers order).
    // Age is how long ago the post was created, so timelines look organic and list ordering/filtering
    // endpoints have something real to work with. Ordered oldest first so ids ascend with time.
    private static readonly (int AuthorIndex, string Content, TimeSpan Age)[] SeedPosts =
    [
        (0, "Première tarte tatin réussie aujourd'hui, elle n'a pas brûlé et tout le monde s'est resservi !", new TimeSpan(12, 0, 0, 0)),
        (1, "J'ai enfin couru 5 km sans m'arrêter ce matin. Il y a trois mois je ne tenais pas 500 mètres.", new TimeSpan(11, 3, 0, 0)),
        (2, "Terminé le roman que je traînais depuis des mois. Petit mais grande satisfaction.", new TimeSpan(10, 20, 0, 0)),
        (3, "J'ai osé prendre la parole en réunion aujourd'hui, et mon idée a été retenue.", new TimeSpan(10, 2, 0, 0)),
        (4, "Deux semaines sans repousser mon réveil. Mes matinées sont tellement plus calmes.", new TimeSpan(9, 0, 0, 0)),
        (5, "Réparé le vélo de ma fille tout seul, elle était aux anges en repartant.", new TimeSpan(8, 6, 0, 0)),
        (0, "Trié tout le garage ce week-end. On peut enfin y garer la voiture !", new TimeSpan(8, 0, 0, 0)),
        (1, "Cuisiné maison toute la semaine, zéro plat à emporter. Le porte-monnaie dit merci.", new TimeSpan(7, 0, 0, 0)),
        (2, "Envoyé la candidature qui me faisait peur depuis un mois. Quoi qu'il arrive, je l'ai fait.", new TimeSpan(6, 4, 0, 0)),
        (3, "Appelé ma grand-mère juste pour prendre des nouvelles. Une heure au téléphone, aucun regret.", new TimeSpan(6, 0, 0, 0)),
        (4, "Fait ma première vraie séance de méditation de dix minutes sans regarder l'heure.", new TimeSpan(5, 0, 0, 0)),
        (5, "Planté mes premières tomates sur le balcon. On verra bien, mais j'y crois !", new TimeSpan(4, 0, 0, 0)),
        (0, "Dit non à une réunion inutile pour finir mon projet. Petite victoire sur mon agenda.", new TimeSpan(3, 0, 0, 0)),
        (2, "Rangé ma boîte mail : de 2 000 à 0 message non lu. Inbox zero, enfin !", new TimeSpan(2, 0, 0, 0)),
        (1, "Bu de l'eau au lieu de soda toute la journée. Ça paraît bête mais j'en suis fier.", new TimeSpan(1, 0, 0, 0)),
        (4, "Terminé la journée avec ma to-do list entièrement cochée. Ça n'arrive jamais !", new TimeSpan(0, 6, 0, 0))
    ];

    public static async Task<List<PostId>> SeedAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds)
    {
        if (await dbContext.Posts.AnyAsync()) return []; // DB has been seeded

        var postIds = await PopulateDataAsync(dbContext, userIds);
        await FastForwardSequenceId(dbContext);
        return postIds;
    }

    private static async Task<List<PostId>> PopulateDataAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds)
    {
        // Use SQL inserts to avoid key generation/conversion issues with value object IDs.
        var now = DateTime.UtcNow;
        var postIds = new List<PostId>();

        for (var i = 0; i < SeedPosts.Length; i++)
        {
            var (authorIndex, content, age) = SeedPosts[i];
            var id = i + 1;

            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO \"Posts\" (\"Id\", \"Content\", \"UserId\", \"CreatedAt\") VALUES ({id}, {content}, {userIds[authorIndex].Value}, {now - age})");

            postIds.Add(PostId.From(id));
        }

        return postIds;
    }

    private static async Task FastForwardSequenceId(PetitesVictoiresDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT setval(pg_get_serial_sequence('\"Posts\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Posts\"), true);"
        );
    }
}
