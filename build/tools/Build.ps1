
$rootDir = (Get-Item $PSScriptRoot).Parent.Parent
$buildDir = (Get-Item $PSScriptRoot).Parent
$slnFile = (Join-Path $rootDir.FullName "src\PCRE.NET.sln")
$outputDir = (Join-Path $buildDir.FullName "output")

Add-Type -AssemblyName "Microsoft.Build.Utilities.v12.0, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
$msbuild = ([Microsoft.Build.Utilities.ToolLocationHelper]::GetPathToBuildToolsFile("msbuild.exe", "12.0"))

New-Item -ItemType Directory -Force -Path $outputDir > $null
Get-ChildItem $outputDir | Remove-Item -Recurse

& $msbuild @( $slnFile, "/t:Rebuild", "/p:Configuration=Release", "/p:Platform=Any CPU", "/m" )
& $msbuild @( $slnFile, "/t:Rebuild", "/p:Configuration=Release", "/p:Platform=x86", "/m" )
& $msbuild @( $slnFile, "/t:Rebuild", "/p:Configuration=Release", "/p:Platform=x64", "/m" )
