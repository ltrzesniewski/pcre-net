using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Running;

namespace PCRE.Benchmarks;

internal class Program
{
    private static int Main(string[] args)
    {
        WriteHeader();

        if (args.Length > 0 && args[0] == "--allocations")
            return AllocationTest.TestAllocations() ? 0 : 1;

        RunBenchmarks(args);
        return 0;
    }

    private static void RunBenchmarks(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

        Console.WriteLine();
        Console.WriteLine("Press enter to exit");

        while (Console.KeyAvailable)
            Console.ReadKey(true);

        Console.ReadLine();
    }

    private static void WriteHeader()
    {
        Console.WriteLine();
        Console.WriteLine($"Framework: {RuntimeInformation.FrameworkDescription} ({(Environment.Is64BitProcess ? 64 : 32)}-bit)");
        Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
        Console.WriteLine($"Architecture: {RuntimeInformation.ProcessArchitecture} process on {RuntimeInformation.OSArchitecture} OS");
#if NET
        Console.WriteLine($"RID: {RuntimeInformation.RuntimeIdentifier}");
#endif
        Console.WriteLine();
    }
}
