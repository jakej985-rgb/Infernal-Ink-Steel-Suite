@echo off
cd /d C:\InfernalInkSteelSuite\publish\Web

set ASPNETCORE_ENVIRONMENT=Development
REM Web probably talks to same DB/API, or uses backend services:
set ASPNETCORE_URLS=http://0.0.0.0:5002

dotnet InfernalInkSteelSuite.Web.dll
