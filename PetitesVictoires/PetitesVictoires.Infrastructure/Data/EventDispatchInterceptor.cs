using Ardalis.SharedKernel;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PetitesVictoires.Infrastructure.Data;

public class EventDispatchInterceptor(IDomainEventDispatcher domainEventDispatcher) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new())
    {
        var context = eventData.Context;
        if (context is not PetitesVictoiresDbContext appDbContext)
            return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);

        var entitiesWithEvents = appDbContext.ChangeTracker.Entries<HasDomainEventsBase>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count != 0)
            .ToArray();

        await domainEventDispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
