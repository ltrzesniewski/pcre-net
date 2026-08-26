#!/usr/bin/env dotnet

// To update PCRE2:
// - Delete the contents of the PCRE directory
// - Extract the PCRE2 release into the PCRE directory
// - Run this script

var rootPath = GetRepositoryRootPath();
var srcDir = Path.Combine(rootPath, "src");

Console.WriteLine($"Updating PCRE2 in repository: {rootPath}");
{
    var pcre2SrcDir = Path.Combine(srcDir, "PCRE", "src");

    File.Copy(
        Path.Combine(pcre2SrcDir, "pcre2.h.generic"),
        Path.Combine(pcre2SrcDir, "pcre2.h"),
        overwrite: true
    );

    File.Copy(
        Path.Combine(pcre2SrcDir, "pcre2_chartables.c.dist"),
        Path.Combine(pcre2SrcDir, "pcre2_chartables.c"),
        overwrite: true
    );

    File.WriteAllText(
        Path.Combine(pcre2SrcDir, "config.h"),
        $"""

         #include "../../PCRE.NET.Native/pcre2config.h"

         {File.ReadAllText(Path.Combine(pcre2SrcDir, "config.h.generic"))}
         """.ReplaceLineEndings("\n")
    );
}

Console.WriteLine("PCRE2 release patched successfully.");

return;

static string GetRepositoryRootPath()
{
    for (var path = Environment.CurrentDirectory; path is not null; path = Path.GetDirectoryName(path))
    {
        if (Directory.Exists(Path.Combine(path, ".git")))
            return path;
    }

    throw new InvalidOperationException("Could not find the repository root path.");
}
