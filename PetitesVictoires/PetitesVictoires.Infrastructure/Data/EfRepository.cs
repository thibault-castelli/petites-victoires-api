using Ardalis.SharedKernel;
using Ardalis.Specification.EntityFrameworkCore;

namespace PetitesVictoires.Infrastructure.Data;

public class EfRepository<T>(PetitesVictoiresDbContext dbContext)
    : RepositoryBase<T>(dbContext), IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
}
