using Ardalis.SharedKernel;

namespace PetitesVictoires.Core.Common;

public abstract class BaseEntity<TId> : HasDomainEventsBase
{
    public TId Id { get; set; } = default!;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    protected void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkSoftDeleted()
    {
        DeletedAt = DateTime.UtcNow;
        MarkUpdated();
    }
}
