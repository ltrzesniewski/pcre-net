@echo off
cd /D "%~dp0"
call ..\src\packages\psake.4.3.2\tools\psake.cmd tools\default.ps1 %*
pause
