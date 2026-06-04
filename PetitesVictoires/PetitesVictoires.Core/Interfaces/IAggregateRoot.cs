namespace PetitesVictoires.Core.Interfaces;

/// <summary>
///     Apply this marker interface only to aggregate root entities.
///     Repositories implementation can use constraints to ensure it only operates on aggregate roots.
/// </summary>
public interface IAggregateRoot;
