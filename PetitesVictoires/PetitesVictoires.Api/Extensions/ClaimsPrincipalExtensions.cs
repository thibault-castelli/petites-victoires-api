using System.Security.Claims;

namespace PetitesVictoires.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal claimsPrincipal)
    {
        public int GetAuthenticatedUserId()
        {
            return claimsPrincipal.TryGetAuthenticatedUserId(out var authenticatedId)
                ? authenticatedId
                : throw new InvalidOperationException("No valid user id claim on the current claim principal.");
        }

        public bool TryGetAuthenticatedUserId(out int authenticatedId)
        {
            return int.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out authenticatedId);
        }
    }
}