using Ardalis.SharedKernel;

namespace PetitesVictoires.Core.Common;

public abstract class BaseEntity<TId> : HasDomainEventsBase, IHasCreationTime
{
    public TId Id { get; set; } = default!;
    public DateTime CreatedAt { get; init; }
}

public interface IHasCreationTime
{
    DateTime CreatedAt { get; }
}

public interface IAuditable
{
    DateTime? UpdatedAt { get; }
    void MarkUpdated();
}

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; }
    void MarkSoftDeleted();
}
