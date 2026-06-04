namespace PetitesVictoires.Core.Common;

public abstract class BaseEntity<TId>
{
    public required TId Id { get; set; }
}
