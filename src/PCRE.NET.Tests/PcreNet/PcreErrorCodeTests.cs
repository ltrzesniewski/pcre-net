using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using PCRE.Internal;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class PcreErrorCodeTests
{
    [Test]
    public void should_have_all_error_codes()
    {
        var codes = typeof(PcreConstants).GetFields(BindingFlags.Public | BindingFlags.Static)
                                         .Where(i => i.Name.StartsWith("ERROR_"))
                                         .Select(i => (name: i.Name, value: (int)i.GetValue(null)!));

        var errors = Enum.GetValues(typeof(PcreErrorCode))
                         .Cast<int>()
                         .ToHashSet();

        var missingCodes = codes.Where(i => !errors.Contains(i.value))
                                .Select(i => i.name)
                                .ToList();

        if (missingCodes.Count > 0)
            Assert.Fail($"Missing {missingCodes.Count} error codes:{Environment.NewLine}{string.Join(Environment.NewLine, missingCodes)}");
    }
}
