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

        context.AddLinkHeader(page: 2, countPerPage: 10, totalPages: 5);

        var link = LinkHeader(context);
        link.ShouldContain("<https://example.com/Posts?page=1&per_page=10>; rel=\"first\"");
        link.ShouldContain("rel=\"prev\"");
        link.ShouldContain("rel=\"next\"");
        link.ShouldContain("rel=\"last\"");
    }

    [Test]
    public void AddLinkHeader_OnFirstPage_OmitsFirstAndPrev()
    {
        var context = ContextForPostsList();

        context.AddLinkHeader(page: 1, countPerPage: 10, totalPages: 5);

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

        context.AddLinkHeader(page: 5, countPerPage: 10, totalPages: 5);

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

        context.AddLinkHeader(page: 1, countPerPage: 10, totalPages: 1);

        context.Response.Headers.ContainsKey("Link").ShouldBeFalse();
    }
}
