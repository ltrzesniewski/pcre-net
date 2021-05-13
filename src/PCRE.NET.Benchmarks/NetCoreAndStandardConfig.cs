using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace PCRE.NET.Benchmarks
{
    internal class NetCoreAndStandardConfig : ManualConfig
    {
        public NetCoreAndStandardConfig()
        {
            AddJob(
                Job.Default.WithId(".NET Core"),
                Job.Default.WithId(".NET Standard").WithArguments(new[] { new MsBuildArgument("/p:ForceNetStandard=true") })
            );
        }
    }
}
