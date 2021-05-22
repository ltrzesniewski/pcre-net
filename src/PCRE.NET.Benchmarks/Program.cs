using System;
using BenchmarkDotNet.Running;

namespace PCRE.NET.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

            Console.WriteLine();
            Console.WriteLine("Press enter to exit");

            while (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.ReadLine();
        }
    }
}
