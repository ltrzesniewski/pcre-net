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

var outDll = outputDir + @"\PCRE.NET.dll";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

// --- Clean ---

Task("Clean-Output")
    .Does(() =>
    {
        Information($"Output: {outputDir}");

        if (DirectoryExists(outputDir))
            CleanDirectory(outputDir);
        else
            CreateDirectory(outputDir);
    });

Task("Clean-LibZ-Cache")
    .Does(() =>
    {
        CleanDirectory(System.IO.Path.GetTempPath(), file => Regex.IsMatch(file.Path.FullPath, @"[/\\][0-9A-Fa-f]{32}\.dll$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase));
    });

Task("Clean")
    .IsDependentOn("Clean-Output")
    .IsDependentOn("Clean-LibZ-Cache")
;

// --- Build ---

void RunBuild(PlatformTarget platform)
{
    MSBuild(slnFile, new MSBuildSettings
    {
        Configuration = configuration,
        PlatformTarget = platform,
        Targets = { "Rebuild" },
        MaxCpuCount = 0
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
    .IsDependentOn("Build-AnyCPU")
    .IsDependentOn("Build-x86")
    .IsDependentOn("Build-x64")
;

// --- Merge ---

Task("Merge")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() => {
        CopyFile(rootDir + $@"\src\PCRE.NET\bin\{configuration}\PCRE.NET.dll", outDll);
        CopyFile(rootDir + $@"\src\PCRE.NET.Wrapper\bin\Win32\{configuration}\PCRE.NET.Wrapper.dll", outputDir + @"\PCRE.NET.Wrapper.x86.dll");
        CopyFile(rootDir + $@"\src\PCRE.NET.Wrapper\bin\x64\{configuration}\PCRE.NET.Wrapper.dll", outputDir + @"\PCRE.NET.Wrapper.x64.dll");
        CopyFile(outputDir + @"\PCRE.NET.Wrapper.x64.dll", outputDir + @"\PCRE.NET.Wrapper.dll");

        StartProcess(
            rootDir + @"\src\packages\LibZ.Bootstrap.1.1.0.2\tools\libz.exe",
            new ProcessSettings()
                .UseWorkingDirectory(outputDir)
                .WithArguments(args => args
                    .Append("inject-dll")
                    .AppendSwitchQuoted("--assembly", outDll)
                    .AppendSwitchQuoted("--include", outputDir + @"\PCRE.NET.Wrapper.x86.dll")
                    .AppendSwitchQuoted("--include", outputDir + @"\PCRE.NET.Wrapper.x64.dll")
                    .AppendSwitchQuoted("--key", rootDir + @"\src\PCRE.NET.snk")
                    .Append("--move")
            )
        );

        DeleteFile(outputDir + @"\PCRE.NET.Wrapper.dll");
    });

// --- Test ---

void RunTest(bool anyCpu)
{
    var platform = anyCpu ? "AnyCPU" : "x86";

    NUnit3(testDir + @"\PCRE.NET.Tests.dll", new NUnit3Settings {
        Framework = "net-4.5",
        ShadowCopy = false,
        WorkingDirectory = testDir,
        Work = testDir,
        X86 = !anyCpu,
        OutputFile = testDir + $@"\Test-{platform}-out.txt",
        ErrorOutputFile = testDir + $@"\Test-{platform}-err.txt"
    });
}

Task("Test-Prepare")
    .IsDependentOn("Merge")
    .Does(() => {
        CreateDirectory(testDir);
        CopyFileToDirectory(outDll, testDir);
        CopyFileToDirectory(rootDir + $@"\src\PCRE.NET.Tests\bin\{configuration}\PCRE.NET.Tests.dll", testDir);
        CopyFileToDirectory(rootDir + $@"\src\PCRE.NET.Tests\bin\{configuration}\NUnit.Framework.dll", testDir);
        CopyDirectory(rootDir + $@"\src\PCRE.NET.Tests\bin\{configuration}\Pcre", testDir + @"\Pcre");
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
        NuGetRestore(slnFile);
    });

Task("NuGet-Pack")
    .IsDependentOn("Merge")
    .Does(() => {
        var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(outDll).FileVersion;
        version = version.Substring(0, version.LastIndexOf("."));

        NuGetPack(
            rootDir + @"\build\tools\PCRE.NET.nuspec",
            new NuGetPackSettings {
                OutputDirectory = outputDir,
                BasePath = rootDir,
                Version = version
            }
        );
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Task("All")
    .IsDependentOn("Test")
    .IsDependentOn("NuGet-Pack")
;

Task("Default")
    .IsDependentOn("All")
;

RunTarget(target);
