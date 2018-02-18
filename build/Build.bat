@echo off
pushd "%~dp0"

powershell -ExecutionPolicy Bypass -File tools\build.ps1 -Script tools\build.cake -Verbosity Diagnostic

popd
pause
