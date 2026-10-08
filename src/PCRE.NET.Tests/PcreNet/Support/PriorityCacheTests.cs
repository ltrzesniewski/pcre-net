using System.Globalization;
using System.Linq;
using NUnit.Framework;
using PCRE.Internal;
using Shouldly;

namespace PCRE.Tests.PcreNet.Support;

[TestFixture]
public class PriorityCacheTests
{
    private PriorityCache<int, string> _cache = default!;

    [SetUp]
    public void Setup()
    {
        _cache = new PriorityCache<int, string>(3, i => i.ToString(CultureInfo.InvariantCulture));
    }

    [Test]
    public void should_store_item()
    {
        _cache.Count.ShouldBe(0);
        _cache.GetOrAdd(42).ShouldBe("42");
        _cache.Count.ShouldBe(1);

        _cache.Select(i => i.Key).ShouldBe([42]);
        _cache.Select(i => i.Value).ShouldBe(["42"]);
    }

    [Test]
    public void should_expire_old_items()
    {
        _cache.GetOrAdd(1);
        _cache.GetOrAdd(2);
        _cache.GetOrAdd(3);
        _cache.GetOrAdd(4);

        _cache.Count.ShouldBe(3);

        _cache.Select(i => i.Key).ShouldBe([4, 3, 2]);
        _cache.Select(i => i.Value).ShouldBe(["4", "3", "2"]);
    }

    [Test]
    public void should_reorder_items()
    {
        _cache.GetOrAdd(1);
        _cache.GetOrAdd(2);
        _cache.GetOrAdd(3);
        _cache.GetOrAdd(4);
        _cache.GetOrAdd(3);

        _cache.Count.ShouldBe(3);

        _cache.Select(i => i.Key).ShouldBe([3, 4, 2]);
        _cache.Select(i => i.Value).ShouldBe(["3", "4", "2"]);
    }

    [Test]
    public void should_handle_zero_size()
    {
        _cache.CacheSize = 0;

        _cache.GetOrAdd(42).ShouldBe("42");
        _cache.Count.ShouldBe(0);
        _cache.ShouldBeEmpty();
    }

    [Test]
    public void should_shrink()
    {
        _cache.GetOrAdd(1);
        _cache.GetOrAdd(2);
        _cache.GetOrAdd(3);

        _cache.CacheSize = 1;

        _cache.Count.ShouldBe(1);
        _cache.Select(i => i.Key).ShouldBe([3]);
        _cache.Select(i => i.Value).ShouldBe(["3"]);

        _cache.CacheSize = 0;

        _cache.Count.ShouldBe(0);
        _cache.ShouldBeEmpty();
    }

    [Test]
    public void should_grow()
    {
        _cache.GetOrAdd(1);
        _cache.GetOrAdd(2);
        _cache.GetOrAdd(3);

        _cache.CacheSize = 4;

        _cache.GetOrAdd(4);

        _cache.Count.ShouldBe(4);

        _cache.Select(i => i.Key).ShouldBe([4, 3, 2, 1]);
        _cache.Select(i => i.Value).ShouldBe(["4", "3", "2", "1"]);
    }

    [Test]
    public void should_handle_concurrency()
    {
        _cache.CacheSize = 10;

        ParallelEnumerable.Range(0, 1000000)
                          .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                          .WithDegreeOfParallelism(20)
                          .ForAll(i => _cache.GetOrAdd(i).ShouldBe(i.ToString(CultureInfo.InvariantCulture)));

        _cache.Count.ShouldBe(10);
    }
}
