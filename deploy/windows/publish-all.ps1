$ErrorActionPreference = "Stop"

Write-Host "=== Infernal Ink & Steel - Publish All ===" -ForegroundColor Cyan

$exitCode = 0

try {
    param(
        [string]$Configuration = "Release",
        [string]$OutputRoot = "C:\InfernalInkSteelSuite\publish"
    )

    # API
    dotnet publish .\InfernalInkSteelSuite.Api\InfernalInkSteelSuite.Api.csproj `
        -c $Configuration -o "$OutputRoot\Api"

    # WEB
    dotnet publish .\InfernalInkSteelSuite.Web\InfernalInkSteelSuite.Web.csproj `
        -c $Configuration -o "$OutputRoot\Web"

    Write-Host "✅ SUCCESS: Published API and Web to $OutputRoot" -ForegroundColor Green
}
catch {
    $exitCode = 1
    Write-Host ""
    Write-Host "❌ FAILED: Publishing failed" -ForegroundColor Red
    Write-Host "Error details:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}
finally {
    # Only pause if user is in an interactive console
    if ($Host.Name -eq "ConsoleHost") {
        Write-Host ""
        Write-Host "Press any key to close this window..." -ForegroundColor Cyan
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }

    exit $exitCode
}
