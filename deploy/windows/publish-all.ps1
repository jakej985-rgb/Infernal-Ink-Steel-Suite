param(
    [string]$Configuration = "Release",
    [string]$OutputRoot = "C:\InfernalInkSteelSuite\publish"
)

$ErrorActionPreference = "Stop"

# API
dotnet publish .\InfernalInkSteelSuite.Api\InfernalInkSteelSuite.Api.csproj `
    -c $Configuration -o "$OutputRoot\Api"

# WEB
dotnet publish .\InfernalInkSteelSuite.Web\InfernalInkSteelSuite.Web.csproj `
    -c $Configuration -o "$OutputRoot\Web"
