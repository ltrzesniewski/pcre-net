using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace PCRE.NET.Benchmarks
{
    internal class NetCoreStandardConfig : ManualConfig
    {
        public NetCoreStandardConfig()
        {
            AddJob(
                Job.Default.WithId(".NET Core"),
                Job.Default.WithId(".NET Standard").WithArguments(new[] { new MsBuildArgument("/p:ForceNetStandard=true") })
            );
        }
    }

    internal class NetCoreStandardFrameworkConfig : ManualConfig
    {
        public NetCoreStandardFrameworkConfig()
        {
            AddJob(
                Job.Default.WithId(".NET Core"),
                Job.Default.WithId(".NET Standard").WithArguments(new[] { new MsBuildArgument("/p:ForceNetStandard=true") }),
                Job.Default.WithId(".NET Framework").WithRuntime(ClrRuntime.Net48)
            );
        }
    }
}
