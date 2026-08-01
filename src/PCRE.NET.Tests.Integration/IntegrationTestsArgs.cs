using System;
using System.Collections.Generic;

namespace PCRE.Tests.Integration;

internal sealed class IntegrationTestsArgs
{
    public static IntegrationTestsArgs Default { get; } = new();

    public bool Aot { get; private set; }
    public bool Build { get; private set; }
    public string? Rid { get; private set; }

    private IntegrationTestsArgs()
    { }

    public static IntegrationTestsArgs Parse(IEnumerable<string> inputArgs)
    {
        var args = new IntegrationTestsArgs();

        foreach (var arg in inputArgs)
        {
            switch (arg)
            {
                case "--aot":
                    args.Aot = true;
                    break;

                case "--build":
                    args.Build = true;
                    break;

                case "--full":
                    args.Aot = true;
                    args.Build = true;
                    break;

                case { } when HasValue("--rid") is { } rid:
                    args.Rid = rid;
                    break;

                default:
                    throw new ArgumentException($"Unknown argument: {arg}");
            }

            continue;

            string? HasValue(string flag)
                => arg.StartsWith($"{flag}=") ? arg.Substring(flag.Length + 1) : null;
        }

        return args;
    }
}
