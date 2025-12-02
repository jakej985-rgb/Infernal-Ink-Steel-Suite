$ErrorActionPreference = "Stop"

Write-Host "=== Infernal Ink & Steel - Start API Dev ===" -ForegroundColor Cyan

$exitCode = 0

try {
    # Set environment variables
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:DB_PATH = "C:\InfernalInkSteelSuite\Data\infernalinksteel.db"
    $env:FILE_ROOT = "C:\InfernalInkSteelSuite\Uploads"
    $env:ASPNETCORE_URLS = "http://0.0.0.0:5001"

    $publishDir = "C:\InfernalInkSteelSuite\publish\Api"
    
    if (-not (Test-Path $publishDir)) {
        throw "Publish directory not found: $publishDir. Please run publish-all.ps1 first."
    }

    Set-Location $publishDir

    Write-Host "Starting API..." -ForegroundColor Cyan
    dotnet InfernalInkSteelSuite.Api.dll

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet process exited with code $LASTEXITCODE"
    }

    Write-Host "✅ SUCCESS: API dev server started on $env:ASPNETCORE_URLS" -ForegroundColor Green
}
catch {
    $exitCode = 1
    Write-Host ""
    Write-Host "❌ FAILED: API failed to start or crashed" -ForegroundColor Red
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
