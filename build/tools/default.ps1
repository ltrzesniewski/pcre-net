
Properties {
	$rootDir = (Get-Item $PSScriptRoot).Parent.Parent.FullName
	$buildDir = (Get-Item $PSScriptRoot).Parent.FullName
	$slnFile = (Join-Path $rootDir "src\PCRE.NET.sln")
	$outputDir = (Join-Path $buildDir "output")
	
	Add-Type -AssemblyName "Microsoft.Build.Utilities.v12.0, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
	$msbuild = ([Microsoft.Build.Utilities.ToolLocationHelper]::GetPathToBuildToolsFile("msbuild.exe", "12.0"))

	$libz = (Join-Path $rootDir "src\packages\LibZ.Bootstrap.1.1.0.2\tools\libz.exe")
	$nunitDir = (Join-Path $rootDir "src\packages\NUnit.Runners.2.6.3\tools")
	$nuget = (Join-Path $rootDir "build\tools\nuget.exe")
	
	$outDll = (Join-Path $outputDir "PCRE.NET.dll")
	$testDir = (Join-Path $outputDir "test")
	$testDll = (Join-Path $testDir "PCRE.NET.Tests.dll")
	$nuspec = (Join-Path $rootDir "build\tools\PCRE.NET.nuspec")

	$config = "Release"
}

FormatTaskName ("`n`n" + ("-"*50) + "`n    {0}`n" +  ("-"*50) + "`n")

Task default -Depends All

Task All -Depends Clean, Build, Merge, Test, NuGet

##### Clean #####

Task Clean {
	Write-Host "Output directory: $outputDir"
	New-Item -ItemType Directory -Force -Path $outputDir > $null
	Get-ChildItem $outputDir | Remove-Item -Recurse	
	Set-Location $outputDir
}

Task Clean-LibZ-Cache {
	Get-ChildItem -Path $env:TEMP -Recurse -Filter "*.dll" | ? { $_.Name -Match "^[0-9A-Fa-f]{32}\.dll$" } | Remove-Item
}

##### Build #####

Function Run-Build([string]$platform) {
	Exec { & $msbuild @( $slnFile, "/t:Rebuild", "/p:Configuration=$config", "/p:Platform=$platform", "/m" ) }
}

Task Build -Depends Build-AnyCPU, Build-x86, Build-x64

Task Build-AnyCPU {
	Run-Build "Any CPU"
}

Task Build-x86 {
	Run-Build "x86"
}

Task Build-x64 {
	Run-Build "x64"
}

##### Merge #####

Task Merge -Depends Clean {
	Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET\bin\$config\PCRE.NET.dll") -Destination $outputDir
	Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Wrapper\bin\Win32\$config\PCRE.NET.Wrapper.dll") -Destination (Join-Path $outputDir "PCRE.NET.Wrapper.x86.dll")
	Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Wrapper\bin\x64\$config\PCRE.NET.Wrapper.dll") -Destination (Join-Path $outputDir "PCRE.NET.Wrapper.x64.dll")
	Copy-Item -Path (Join-Path $outputDir "PCRE.NET.Wrapper.x64.dll") -Destination (Join-Path $outputDir "PCRE.NET.Wrapper.dll")

	Exec { & $libz @(
		"inject-dll",
		"--assembly", (Join-Path $outputDir "PCRE.NET.dll"),
		"--include", (Join-Path $outputDir "PCRE.NET.Wrapper.x86.dll"),
		"--include", (Join-Path $outputDir "PCRE.NET.Wrapper.x64.dll"),
		"--key", (Join-Path $rootDir "src\PCRE.NET.snk"),
		"--move"
	) }

	Remove-Item (Join-Path $outputDir "PCRE.NET.Wrapper.dll")
}

##### Test #####

Function Run-Test([string]$runner, [string]$platform) {
	Exec { & (Join-Path $nunitDir $runner) @(
		$testDll,
		"/framework:net-4.5",
		"/noshadow",
		"/work:$testDir",
		"/result:Test-$platform.xml",
		"/out:Test-$platform-out.txt",
		"/err:Test-$platform-err.txt"
	) }
}

Task Test -Depends Test-AnyCPU, Test-x86

Task Test-Init -Depends Merge, Clean-LibZ-Cache {
	New-Item -ItemType Directory -Path $testDir > $null

	Copy-Item -Path $outDll -Destination $testDir
	Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Tests\bin\$config\PCRE.NET.Tests.dll") -Destination $testDir
	Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Tests\bin\$config\NUnit.Framework.dll") -Destination $testDir
	Copy-Item -Path (Join-Path $rootDir "src\PCRE.NET.Tests\bin\$config\Pcre") -Destination $testDir -Recurse
}

Task Test-AnyCPU -Depends Test-Init {
	Run-Test "nunit-console.exe" "AnyCPU"
}

Task Test-x86 -Depends Test-Init {
	Run-Test "nunit-console-x86.exe" "x86"
}

##### NuGet #####

Task NuGet -Depends Merge {

	$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($outDll).FileVersion
	$version = $version.Substring(0, $version.LastIndexOf("."))

	Exec { & $nuget @(
		"Pack", $nuspec,
		"-OutputDirectory", $outputDir,
		"-BasePath", $rootDir,
		"-Version", $version
		"-NonInteractive"
	) }
}
