using Microsoft.AspNetCore.Http;
using PetitesVictoires.Api.Extensions;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Extensions;

[TestFixture]
public class HttpContextLinkHeaderTests
{
    private static DefaultHttpContext ContextForPostsList()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/Posts";
        return context;
    }

    private static string LinkHeader(DefaultHttpContext context)
    {
        return context.Response.Headers["Link"].ToString();
    }

    [Test]
    public void AddLinkHeader_OnMiddlePage_IncludesFirstPrevNextLastWithBuiltUrls()
    {
        var context = ContextForPostsList();

        context.AddLinkHeader(2, 10, 5);

        var link = LinkHeader(context);
        link.ShouldContain("<https://example.com/Posts?page=1&count_per_page=10>; rel=\"first\"");
        link.ShouldContain("rel=\"prev\"");
        link.ShouldContain("rel=\"next\"");
        link.ShouldContain("rel=\"last\"");
    }

    [Test]
    public void AddLinkHeader_OnFirstPage_OmitsFirstAndPrev()
    {
        var context = ContextForPostsList();

        context.AddLinkHeader(1, 10, 5);

        var link = LinkHeader(context);
        link.ShouldNotContain("rel=\"first\"");
        link.ShouldNotContain("rel=\"prev\"");
        link.ShouldContain("rel=\"next\"");
        link.ShouldContain("rel=\"last\"");
    }

    [Test]
    public void AddLinkHeader_OnLastPage_OmitsNextAndLast()
    {
        var context = ContextForPostsList();

        context.AddLinkHeader(5, 10, 5);

        var link = LinkHeader(context);
        link.ShouldContain("rel=\"first\"");
        link.ShouldContain("rel=\"prev\"");
        link.ShouldNotContain("rel=\"next\"");
        link.ShouldNotContain("rel=\"last\"");
    }

    [Test]
    public void AddLinkHeader_WhenOnlyOnePage_DoesNotSetAnyHeader()
    {
        var context = ContextForPostsList();

        context.AddLinkHeader(1, 10, 1);

        context.Response.Headers.ContainsKey("Link").ShouldBeFalse();
    }
}
