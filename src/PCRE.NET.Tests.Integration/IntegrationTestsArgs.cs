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
            if (arg.IndexOf('=') is var equalIndex and >= 0)
            {
                var flag = arg.Substring(0, equalIndex);
                var value = arg.Substring(equalIndex + 1);

                switch (flag)
                {
                    case "--rid":
                        args.Rid = value;
                        break;

                    case "--net":
                        args.Net = Enum.TryParse<NetType>(value, true, out var parsedNet) || Enum.TryParse($"Net{value}", true, out parsedNet) ? parsedNet : InvalidEnumValue<NetType>();
                        break;

                    case "--arch":
                        args.Arch = Enum.TryParse<Architecture>(value, true, out var parsedArch) ? parsedArch : InvalidEnumValue<Architecture>();
                        break;

                    default:
                        throw new ArgumentException($"Unknown argument: {arg}");
                }
            }
            else
            {
                switch (arg)
                {
                    case "--aot":
                        args.Aot = true;
                        break;

                    case "--nuget":
                        args.NuGet = true;
                        break;

                    default:
                        throw new ArgumentException($"Unknown argument: {arg}");
                }
            }
        }

        return args;
    }

    private static T InvalidEnumValue<T>()
        where T : struct, Enum
        => (T)(object)int.MinValue;

    public static string Display(string? value)
        => value ?? "<not provided>";

    public static string Display<T>(T? value)
        where T : struct, Enum
        => value is { } providedValue
            ? !providedValue.Equals(InvalidEnumValue<T>())
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
