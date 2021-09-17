using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class PcreMatchBufferTests
    {
        [Test]
        public void should_use_match_buffer()
        {
            var re = new PcreRegex("foo");
            var buffer = re.CreateMatchBuffer();

            var match = buffer.Match("foo".AsSpan());

            Assert.That(Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]), Is.True);
        }

        [Test]
        public void should_use_match_buffer_for_no_match()
        {
            var re = new PcreRegex("foo");
            var buffer = re.CreateMatchBuffer();

            var match = buffer.Match("bar".AsSpan());

            Assert.That(Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]), Is.True);
        }

        [Test]
        public void should_use_callout_buffer()
        {
            var re = new PcreRegex(@"f(o)(?C1)o");
            var buffer = re.CreateMatchBuffer();

            var match = buffer.Match("foo".AsSpan(), data =>
            {
                Assert.That(data.Match.Value.ToString(), Is.EqualTo("fo"));
                Assert.That(data.Match[1].Value.ToString(), Is.EqualTo("o"));

                Assert.That(Unsafe.AreSame(ref MemoryMarshal.GetReference(data.OutputVector), ref buffer.CalloutOutputVector[0]), Is.True);

                return PcreCalloutResult.Pass;
            });

            Assert.That(match.Success, Is.True);
        }
    }
}
