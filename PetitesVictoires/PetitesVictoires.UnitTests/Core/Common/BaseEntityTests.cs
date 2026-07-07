using PetitesVictoires.Core.Common;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.Common;

[TestFixture]
public class BaseEntityTests
{
    // Smallest possible concrete BaseEntity so we can exercise the base behavior in isolation.
    private sealed class TestEntity : BaseEntity<int>
    {
    }

    [Test]
    public void NewEntity_HasNoUpdatedOrDeletedTimestamp()
    {
        var entity = new TestEntity();

        entity.UpdatedAt.ShouldBeNull();
        entity.DeletedAt.ShouldBeNull();
    }

    [Test]
    public void MarkUpdated_SetsUpdatedAtToUtcNow()
    {
        var entity = new TestEntity();

        var before = DateTime.UtcNow;
        entity.MarkUpdated();

        entity.UpdatedAt.ShouldNotBeNull();
        entity.UpdatedAt!.Value.ShouldBeInRange(before, DateTime.UtcNow);
    }

    [Test]
    public void MarkSoftDeleted_SetsDeletedAtToUtcNow()
    {
        var entity = new TestEntity();

        var before = DateTime.UtcNow;
        entity.MarkSoftDeleted();

        entity.DeletedAt.ShouldNotBeNull();
        entity.DeletedAt!.Value.ShouldBeInRange(before, DateTime.UtcNow);
    }
}
