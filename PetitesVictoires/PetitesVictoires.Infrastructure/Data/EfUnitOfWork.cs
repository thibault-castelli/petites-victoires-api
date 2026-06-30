using Microsoft.EntityFrameworkCore.Storage;
using PetitesVictoires.Core.Interfaces;

namespace PetitesVictoires.Infrastructure.Data;

internal sealed class EfUnitOfWork(PetitesVictoiresDbContext dbContext) : IUnitOfWork
{
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransaction(transaction);
    }

    private sealed class EfTransaction(IDbContextTransaction transaction) : ITransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return transaction.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return transaction.RollbackAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return transaction.DisposeAsync();
        }
    }
}
