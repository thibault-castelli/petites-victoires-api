using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PetitesVictoires.Core.Common;

namespace PetitesVictoires.Infrastructure.Data;

public class AuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Modified, Entity: IAuditable auditable })
                auditable.MarkUpdated();

            if (entry is { State: EntityState.Deleted, Entity: ISoftDeletable softDeletable })
            {
                entry.State = EntityState.Modified; // convert hard delete → update
                softDeletable.MarkSoftDeleted();
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
