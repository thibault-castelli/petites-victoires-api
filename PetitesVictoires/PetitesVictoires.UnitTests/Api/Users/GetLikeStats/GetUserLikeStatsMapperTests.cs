using PetitesVictoires.Api.Users.GetLikeStats;
using PetitesVictoires.UseCases.Users;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Users.GetLikeStats;

[TestFixture]
public class GetUserLikeStatsMapperTests
{
    [Test]
    public void FromEntity_CopiesEachCountIntoTheMatchingRecordField()
    {
        var dto = new UserLikeStatsDto(3, 7);

        var record = new GetUserLikeStatsMapper().FromEntity(dto);

        record.LikesGiven.ShouldBe(3);
        record.LikesReceived.ShouldBe(7);
    }
}
