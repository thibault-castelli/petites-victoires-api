using System.Text;
using System.Text.Json;
using Ardalis.Result;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using PetitesVictoires.UseCases.Common;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Common;

[TestFixture]
public class CachingBehaviorTests
{
    private const string TestCacheKey = "test-cache-key";
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);

    private IDistributedCache _cache = null!;
    private CachingBehavior<TestCachedQuery, Result<TestDto>> _behavior = null!;
    private int _nextCallCount;

    [SetUp]
    public void SetUp()
    {
        _cache = Substitute.For<IDistributedCache>();
        // Unconfigured, NSubstitute returns an empty byte[] rather than null; a real cache returns null on a miss.
        _cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((byte[]?)null);
        var logger = NullLogger<CachingBehavior<TestCachedQuery, Result<TestDto>>>.Instance;
        _behavior = new CachingBehavior<TestCachedQuery, Result<TestDto>>(_cache, logger);
        _nextCallCount = 0;
    }

    private static TestCachedQuery Query(TimeSpan? timeout = null)
    {
        return new TestCachedQuery(timeout);
    }

    private static Result<TestDto> OkResponse()
    {
        return Result<TestDto>.Success(new TestDto(1, "ok"));
    }

    private static Result<TestDto> NotFoundResponse()
    {
        return Result<TestDto>.NotFound();
    }

    private MessageHandlerDelegate<TestCachedQuery, Result<TestDto>> Next(Result<TestDto> response)
    {
        return (_, _) =>
        {
            _nextCallCount++;
            return new ValueTask<Result<TestDto>>(response);
        };
    }

    // GetStringAsync/SetStringAsync are extension methods and cannot be substituted directly,
    // so the underlying GetAsync/SetAsync they delegate to are arranged/verified instead.
    private void ArrangeCacheHit(Result<TestDto> response)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        _cache.GetAsync(TestCacheKey, Arg.Any<CancellationToken>()).Returns(bytes);
    }

    [Test]
    public async Task Handle_WhenMessageIsNotCachedQuery_CallsNextWithoutTouchingCache()
    {
        var logger = NullLogger<CachingBehavior<TestPlainQuery, Result<TestDto>>>.Instance;
        var behavior = new CachingBehavior<TestPlainQuery, Result<TestDto>>(_cache, logger);
        var called = false;
        MessageHandlerDelegate<TestPlainQuery, Result<TestDto>> next = (_, _) =>
        {
            called = true;
            return new ValueTask<Result<TestDto>>(OkResponse());
        };

        var result = await behavior.Handle(new TestPlainQuery(), next, CancellationToken.None);

        called.ShouldBeTrue();
        result.IsSuccess.ShouldBeTrue();
        await _cache.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cache.DidNotReceive()
            .SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenCacheHit_ReturnsCachedResponse()
    {
        ArrangeCacheHit(OkResponse());

        var result = await _behavior.Handle(Query(), Next(OkResponse()), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(1);
    }

    [Test]
    public async Task Handle_WhenCacheHit_DoesNotCallNext()
    {
        ArrangeCacheHit(OkResponse());

        await _behavior.Handle(Query(), Next(OkResponse()), CancellationToken.None);

        _nextCallCount.ShouldBe(0);
    }

    [Test]
    public async Task Handle_WhenCacheHit_DoesNotWriteToCache()
    {
        ArrangeCacheHit(OkResponse());

        await _behavior.Handle(Query(), Next(OkResponse()), CancellationToken.None);

        await _cache.DidNotReceive()
            .SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenCacheMissAndResultIsOk_CallsNext()
    {
        await _behavior.Handle(Query(), Next(OkResponse()), CancellationToken.None);

        _nextCallCount.ShouldBe(1);
    }

    [Test]
    public async Task Handle_WhenCacheMissAndResultIsOk_ReturnsResponse()
    {
        var result = await _behavior.Handle(Query(), Next(OkResponse()), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(1);
    }

    [Test]
    public async Task Handle_WhenCacheMissAndResultIsOk_CachesResponse()
    {
        await _behavior.Handle(Query(), Next(OkResponse()), CancellationToken.None);

        await _cache.Received(1)
            .SetAsync(TestCacheKey, Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenCacheMissAndResultIsNotOk_DoesNotCacheResponse()
    {
        await _behavior.Handle(Query(), Next(NotFoundResponse()), CancellationToken.None);

        await _cache.DidNotReceive()
            .SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenResultIsNotOk_ReturnsResponse()
    {
        var result = await _behavior.Handle(Query(), Next(NotFoundResponse()), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenCachingWithQueryTimeout_UsesQueryTimeout()
    {
        var timeout = TimeSpan.FromMinutes(10);

        await _behavior.Handle(Query(timeout), Next(OkResponse()), CancellationToken.None);

        await _cache.Received(1)
            .SetAsync(TestCacheKey, Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == timeout),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenCachingWithoutQueryTimeout_UsesDefaultTimeout()
    {
        await _behavior.Handle(Query(), Next(OkResponse()), CancellationToken.None);

        await _cache.Received(1)
            .SetAsync(TestCacheKey, Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == DefaultTimeout),
                Arg.Any<CancellationToken>());
    }

    private sealed record TestDto(int Id, string Name);

    private sealed record TestCachedQuery(TimeSpan? Timeout) : ICachedQuery<Result<TestDto>>
    {
        public string CacheKey => TestCacheKey;
        public TimeSpan? CacheTimeout => Timeout;
    }

    private sealed record TestPlainQuery : IQuery<Result<TestDto>>;
}
