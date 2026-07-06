namespace PetitesVictoires.Api.Extensions;

public static class HttpContext
{
    extension(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        public void AddLinkHeader(int page, int countPerPage, int totalPages)
        {
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}";

            string Link(string rel, int p)
            {
                return $"<{baseUrl}?page={p}&per_page={countPerPage}>; rel=\"{rel}\"";
            }

            var parts = new List<string>();
            if (page > 1)
            {
                parts.Add(Link("first", 1));
                parts.Add(Link("prev", page - 1));
            }

            if (page < totalPages)
            {
                parts.Add(Link("next", page + 1));
                parts.Add(Link("last", totalPages));
            }

            if (parts.Count > 0)
                httpContext.Response.Headers["Link"] = string.Join(", ", parts);
        }
    }
}
