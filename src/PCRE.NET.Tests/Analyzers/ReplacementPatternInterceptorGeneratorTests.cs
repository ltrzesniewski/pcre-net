using System.Threading.Tasks;
using NUnit.Framework;
using PCRE.NET.Analyzers;

namespace PCRE.Tests.Analyzers;

[TestFixture]
public class ReplacementPattern : BaseInterceptorTests<ReplacementPatternInterceptorGenerator>
{
    [Test]
    public Task generates_intercepts_with_literals()
    {
        return Verify(
            """
            using PCRE;

            class C
            {
                void M()
                {
                    var regex = new PcreRegex("foo(?<group>bar)baz");

                    _ = regex.Replace("subject", "");
                    _ = regex.Replace("subject", "replacement");
                    _ = regex.Replace("subject", "a $$ b");
                    _ = regex.Replace("subject", "a $& b");
                    _ = regex.Replace("subject", "a $0 b");
                    _ = regex.Replace("subject", "a $1 b");
                    _ = regex.Replace("subject", "a $2 b");
                    _ = regex.Replace("subject", "a ${group} b");
                    _ = regex.Replace("subject", "a ${other} b");
                    _ = regex.Replace("subject", "a $` b");
                    _ = regex.Replace("subject", "a $' b");
                    _ = regex.Replace("subject", "a $_ b");
                    _ = regex.Replace("subject", "a $+ b");
                }
            }
            """
        );
    }
}
