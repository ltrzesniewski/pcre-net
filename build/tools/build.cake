#tool nuget:?package=NUnit.ConsoleRunner&version=3.8.0

using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PATHS
//////////////////////////////////////////////////////////////////////

var rootDir = MakeAbsolute(Directory(@"..\.."));
var outputDir = MakeAbsolute(Directory(@"..\output"));
var testDir = outputDir + @"\test";

var slnFile = rootDir + @"\src\PCRE.NET.sln";
var libProj = rootDir + @"\src\PCRE.NET\PCRE.NET.csproj";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

// --- Clean ---

Task("Clean")
    .Does(() =>
    {
        Information($"Output: {outputDir}");

        if (DirectoryExists(outputDir))
            CleanDirectory(outputDir);
        else
            CreateDirectory(outputDir);
    });

// --- Build ---

void RunBuild(PlatformTarget platform)
{
    MSBuild(slnFile, new MSBuildSettings
    {
        Configuration = configuration,
        PlatformTarget = platform,
        Targets = { "Rebuild" },
        MaxCpuCount = 0,
        Verbosity = Verbosity.Minimal
    });
}

Task("Build-AnyCPU")
    .IsDependentOn("NuGet-Restore")
    .Does(() => RunBuild(PlatformTarget.MSIL));

Task("Build-x86")
    .IsDependentOn("NuGet-Restore")
    .Does(() => RunBuild(PlatformTarget.x86));

Task("Build-x64")
    .IsDependentOn("NuGet-Restore")
    .Does(() => RunBuild(PlatformTarget.x64));

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Build-AnyCPU")
    .IsDependentOn("Build-x86")
    .IsDependentOn("Build-x64")
;

// --- Test ---

void RunTest(bool anyCpu)
{
    var platform = anyCpu ? "AnyCPU" : "x86";

    NUnit3($@"{rootDir}\src\PCRE.NET.Tests\bin\{configuration}\net472\PCRE.NET.Tests.dll", new NUnit3Settings {
        ShadowCopy = false,
        X86 = !anyCpu,
        OutputFile = testDir + $@"\Test-{platform}-out.txt",
        Work = testDir
    });
}

Task("Test-Prepare")
    .IsDependentOn("Build")
    .Does(() => {
        CreateDirectory(testDir);
    });

Task("Test-AnyCPU")
    .IsDependentOn("Test-Prepare")
    .Does(() => RunTest(true));

Task("Test-x86")
    .IsDependentOn("Test-Prepare")
    .Does(() => RunTest(false));

Task("Test")
    .IsDependentOn("Test-AnyCPU")
    .IsDependentOn("Test-x86")
;

// --- NuGet ---

Task("NuGet-Restore")
    .Does(() => {
        DotNetCoreRestore(slnFile);
    });

Task("NuGet-Pack")
    .Does(() => {
        MSBuild($@"{rootDir}\src\PCRE.NET\PCRE.NET.csproj", new MSBuildSettings
        {
            Configuration = configuration,
            Targets = { "Pack" },
            Properties = {
                ["PackageOutputPath"] = new[] { outputDir.FullPath }
            },
            MaxCpuCount = 0,
            Verbosity = Verbosity.Minimal
        });

        Warning("");
        Warning("==============================================================");
        Warning("| WARNING: This build script produces a Windows-only package |");
        Warning("==============================================================");
        Warning("");
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Task("All")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("NuGet-Pack")
;

Task("Default")
    .IsDependentOn("All")
;

RunTarget(target);
