@echo off
cd /d C:\InfernalInkSteelSuite\publish\Api

REM Dev environment
set ASPNETCORE_ENVIRONMENT=Development
set DB_PATH=C:\InfernalInkSteelSuite\Data\infernalinksteel.db
set FILE_ROOT=C:\InfernalInkSteelSuite\Uploads
set ASPNETCORE_URLS=http://0.0.0.0:5001

dotnet InfernalInkSteelSuite.Api.dll
