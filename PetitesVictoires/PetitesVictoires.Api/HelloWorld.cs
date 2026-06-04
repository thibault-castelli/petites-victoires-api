using FastEndpoints;

namespace PetitesVictoires.Api;

public class HelloWorld : EndpointWithoutRequest<HelloWorldResponse>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new HelloWorldResponse { Text = "Hello World!" };
        await Send.OkAsync(response, ct);
    }
}

public class HelloWorldResponse
{
    public string Text { get; set; }
}
