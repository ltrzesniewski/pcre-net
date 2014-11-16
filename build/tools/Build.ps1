
# This is a quickly hacked file to get the job done
# TODO : Convert to psake

$rootDir = (Get-Item $PSScriptRoot).Parent.Parent.FullName
$buildDir = (Get-Item $PSScriptRoot).Parent.FullName
$slnFile = (Join-Path $rootDir "src\PCRE.NET.sln")
$outputDir = (Join-Path $buildDir "output")

Add-Type -AssemblyName "Microsoft.Build.Utilities.v12.0, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
$msbuild = ([Microsoft.Build.Utilities.ToolLocationHelper]::GetPathToBuildToolsFile("msbuild.exe", "12.0"))

$libz = (Join-Path $rootDir "src\packages\LibZ.Bootstrap.1.1.0.2\tools\libz.exe")
$nunitDir = (Join-Path $rootDir "src\packages\NUnit.Runners.2.6.3\tools")

# Create output dir

New-Item -ItemType Directory -Force -Path $outputDir > $null
Get-ChildItem $outputDir | Remove-Item -Recurse
Set-Location $outputDir

function Run([scriptblock]$code)
{
    & $code
    if ($LastExitCode -ne 0) {
        throw "Command failed"
    }
}

# Rebuild

Run { & $msbuild @( $slnFile, "/t:Rebuild", "/p:Configuration=Release", "/p:Platform=Any CPU", "/m" ) }
Run { & $msbuild @( $slnFile, "/t:Rebuild", "/p:Configuration=Release", "/p:Platform=x86", "/m" ) }
Run { & $msbuild @( $slnFile, "/t:Rebuild", "/p:Configuration=Release", "/p:Platform=x64", "/m" ) }

# LibZ

Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET\bin\Release\PCRE.NET.dll") -Destination $outputDir
Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Wrapper\bin\Win32\Release\PCRE.NET.Wrapper.dll") -Destination (Join-Path $outputDir "PCRE.NET.Wrapper.x86.dll")
Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Wrapper\bin\x64\Release\PCRE.NET.Wrapper.dll") -Destination (Join-Path $outputDir "PCRE.NET.Wrapper.x64.dll")
Copy-Item -Path (Join-Path $outputDir "PCRE.NET.Wrapper.x64.dll") -Destination (Join-Path $outputDir "PCRE.NET.Wrapper.dll")

Run { & $libz @(
    "inject-dll",
    "--assembly", (Join-Path $outputDir "PCRE.NET.dll"),
    "--include", (Join-Path $outputDir "PCRE.NET.Wrapper.x86.dll"),
    "--include", (Join-Path $outputDir "PCRE.NET.Wrapper.x64.dll"),
    "--key", (Join-Path $rootDir "src\PCRE.NET.snk"),
    "--move"
) }

Remove-Item (Join-Path $outputDir "PCRE.NET.Wrapper.dll")
$outDll = (Join-Path $outputDir "PCRE.NET.dll")

# Test

$testDir = (Join-Path $outputDir "test")
New-Item -ItemType Directory -Path $testDir > $null
Set-Location $testDir

Copy-Item -Path $outDll -Destination $testDir
Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Tests\bin\Release\PCRE.NET.Tests.dll") -Destination $testDir
Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Tests\bin\Release\NUnit.Framework.dll") -Destination $testDir
Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Tests\bin\Release\Pcre") -Destination $testDir -Recurse

$testDll = (Join-Path $testDir "PCRE.NET.Tests.dll")

Run { & (Join-Path $nunitDir "nunit-console.exe") @( $testDll, "/framework:net-4.5", "/out:Test-x64.txt", "/err:Test-x64-err.txt", "/noshadow", "/stoponerror") }
Run { & (Join-Path $nunitDir "nunit-console-x86.exe") @($testDll, "/framework:net-4.5", "/out:Test-x86.txt", "/err:Test-x86-err.txt", "/noshadow", "/stoponerror") }
