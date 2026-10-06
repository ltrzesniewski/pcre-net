using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Shouldly;

// ReSharper disable CheckNamespace

[SetUpFixture]
public static class Global
{
    [OneTimeSetUp]
    public static void GlobalSetup()
    {
        TestContext.Progress.WriteLine(
            $"""

             PCRE.NET Tests

               Framework: {RuntimeInformation.FrameworkDescription} ({(Environment.Is64BitProcess ? 64 : 32)}-bit)
               Operating System: {RuntimeInformation.OSDescription}
               Architecture: {RuntimeInformation.ProcessArchitecture} process on {RuntimeInformation.OSArchitecture} OS
             """
        );

#if NET
        TestContext.Progress.WriteLine(
            $"""
              Runtime Identifier: {RuntimeInformation.RuntimeIdentifier}
            """
        );
#endif

        TestContext.Progress.WriteLine(
            """

           ---

           """
        );

        // Uncomment to enable diffs
        // ShouldMatchConfiguration.ShouldMatchApprovedDefaults.ConfigureDiffEngine();
    }
}
