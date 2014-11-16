@echo off
pushd "%~dp0"

.\tools\nuget.exe restore ..\src\PCRE.NET.sln
if errorlevel 1 goto end

call ..\src\packages\psake.4.3.2\tools\psake.cmd .\tools\default.ps1 %*

:end
popd
pause
