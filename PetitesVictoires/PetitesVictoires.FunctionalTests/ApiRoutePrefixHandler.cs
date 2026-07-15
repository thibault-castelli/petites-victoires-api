namespace PetitesVictoires.FunctionalTests;

/// <summary>
///     The app registers every endpoint under the global "api" route prefix (UseFastEndpoints in
///     MiddlewareConfigurations). Route constants deliberately don't carry it, so apply it here —
///     one place in the tests mirroring the one place in the app.
/// </summary>
public sealed class ApiRoutePrefixHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uri = request.RequestUri!;
        if (!uri.AbsolutePath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            request.RequestUri = new Uri(uri, $"/api{uri.PathAndQuery}");

        return base.SendAsync(request, cancellationToken);
    }
}
