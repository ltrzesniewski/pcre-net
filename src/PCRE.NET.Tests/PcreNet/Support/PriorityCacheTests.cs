using System.Globalization;
using System.Linq;
using NUnit.Framework;
using PCRE.Internal;

namespace PCRE.Tests.PcreNet.Support
{
    [TestFixture]
    public class PriorityCacheTests
    {
        private PriorityCache<int, string> _cache;

        [SetUp]
        public void Setup()
        {
            _cache = new PriorityCache<int, string>(3, i => i.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void should_store_item()
        {
            Assert.That(_cache.Count, Is.EqualTo(0));
            Assert.That(_cache.GetOrAdd(42), Is.EqualTo("42"));
            Assert.That(_cache.Count, Is.EqualTo(1));

            Assert.That(_cache.Select(i => i.Key), Is.EqualTo(new[] { 42 }));
            Assert.That(_cache.Select(i => i.Value), Is.EqualTo(new[] { "42" }));
        }

        [Test]
        public void should_expire_old_items()
        {
            _cache.GetOrAdd(1);
            _cache.GetOrAdd(2);
            _cache.GetOrAdd(3);
            _cache.GetOrAdd(4);

            Assert.That(_cache.Count, Is.EqualTo(3));

            Assert.That(_cache.Select(i => i.Key), Is.EqualTo(new[] { 4, 3, 2 }));
            Assert.That(_cache.Select(i => i.Value), Is.EqualTo(new[] { "4", "3", "2" }));
        }

        [Test]
        public void should_reorder_items()
        {
            _cache.GetOrAdd(1);
            _cache.GetOrAdd(2);
            _cache.GetOrAdd(3);
            _cache.GetOrAdd(4);
            _cache.GetOrAdd(3);

            Assert.That(_cache.Count, Is.EqualTo(3));

            Assert.That(_cache.Select(i => i.Key), Is.EqualTo(new[] { 3, 4, 2 }));
            Assert.That(_cache.Select(i => i.Value), Is.EqualTo(new[] { "3", "4", "2" }));
        }

        [Test]
        public void should_handle_zero_size()
        {
            _cache.CacheSize = 0;

            Assert.That(_cache.GetOrAdd(42), Is.EqualTo("42"));
            Assert.That(_cache.Count, Is.EqualTo(0));
            Assert.That(_cache, Is.Empty);
        }

        [Test]
        public void should_shrink()
        {
            _cache.GetOrAdd(1);
            _cache.GetOrAdd(2);
            _cache.GetOrAdd(3);

            _cache.CacheSize = 1;

            Assert.That(_cache.Count, Is.EqualTo(1));
            Assert.That(_cache.Select(i => i.Key), Is.EqualTo(new[] { 3 }));
            Assert.That(_cache.Select(i => i.Value), Is.EqualTo(new[] { "3" }));

            _cache.CacheSize = 0;

            Assert.That(_cache.Count, Is.EqualTo(0));
            Assert.That(_cache, Is.Empty);
        }

        [Test]
        public void should_grow()
        {
            _cache.GetOrAdd(1);
            _cache.GetOrAdd(2);
            _cache.GetOrAdd(3);

            _cache.CacheSize = 4;

            _cache.GetOrAdd(4);

            Assert.That(_cache.Count, Is.EqualTo(4));

            Assert.That(_cache.Select(i => i.Key), Is.EqualTo(new[] { 4, 3, 2, 1 }));
            Assert.That(_cache.Select(i => i.Value), Is.EqualTo(new[] { "4", "3", "2", "1" }));
        }

        [Test]
        public void should_handle_concurrency()
        {
            _cache.CacheSize = 10;

            ParallelEnumerable.Range(0, 1000000)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithDegreeOfParallelism(20)
                .ForAll(i => Assert.That(_cache.GetOrAdd(i), Is.EqualTo(i.ToString(CultureInfo.InvariantCulture))));

            Assert.That(_cache.Count, Is.EqualTo(10));
        }
    }
}
