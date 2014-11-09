@echo off
cd /D "%~dp0"
powershell -NoLogo -NoProfile -ExecutionPolicy RemoteSigned -File tools\Build.ps1
pause
