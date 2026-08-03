using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PCRE.Tests.Integration;

internal sealed class IntegrationTestsArgs
{
    public static IntegrationTestsArgs Default { get; } = new();

    public bool Aot { get; private set; }
    public bool NuGet { get; private set; }
    public string? Rid { get; private set; }
    public NetType? Net { get; private set; }
    public Architecture? Arch { get; private set; }

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

                case "--nuget":
                    args.NuGet = true;
                    break;

                case { } when HasValue("--rid") is { } rid:
                    args.Rid = rid;
                    break;

                case { } when HasValue("--net") is { } net:
                    args.Net = Enum.TryParse<NetType>(net, true, out var parsedNet) || Enum.TryParse($"Net{net}", true, out parsedNet) ? parsedNet : InvalidValue<NetType>();
                    break;

                case { } when HasValue("--arch") is { } arch:
                    args.Arch = Enum.TryParse<Architecture>(arch, true, out var parsedArch) ? parsedArch : InvalidValue<Architecture>();
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

    private static T InvalidValue<T>()
        where T : struct, Enum
        => (T)(object)int.MinValue;

    public static string Display(string? value)
        => value ?? "<not provided>";

    public static string Display<T>(T? value)
        where T : struct, Enum
        => value is { } providedValue
            ? !providedValue.Equals(InvalidValue<T>())
                ? providedValue.ToString()
                : "<invalid>"
            : "<not provided>";

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum NetType
    {
        None,
        NetCore,
        NetFramework
    }
}
