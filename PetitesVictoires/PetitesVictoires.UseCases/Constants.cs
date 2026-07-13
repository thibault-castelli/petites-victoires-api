namespace PetitesVictoires.UseCases;

public static class Constants
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
    public const int MaxCachedPage = 3;

    public const string PostCachePrefix = "post:";
    public const string UserCachePrefix = "user:";
    public const string UserLikeStatsCachePrefix = "like-stats:";
}
