using System;
using BenchmarkDotNet.Running;

namespace PCRE.NET.Benchmarks;

internal class Program
{
    private static int Main(string[] args)
    {
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
}
