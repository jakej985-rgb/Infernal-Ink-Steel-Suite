# Setup-local-env.ps1
# Sets up the local development environment for the Infernal Ink & Steel Suite

$ErrorActionPreference = "Stop"

$scriptPath = $PSScriptRoot
$projectRoot = Join-Path $scriptPath "..\.."
$certsDir = Join-Path $projectRoot "certs"

Write-Host "=== Setting up Local Dev Environment ===" -ForegroundColor Cyan

# Check for certs directory (Audit Fix #20)
if (-not (Test-Path -Path $certsDir)) {
    Write-Host "Creating missing certs directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $certsDir | Out-Null
} else {
    Write-Host "Certs directory already exists." -ForegroundColor Green
}

# Generate dev certificate if missing (example logic)
$certFile = Join-Path $certsDir "aspnetapp.pfx"
if (-not (Test-Path -Path $certFile)) {
    Write-Host "Generating development certificate..." -ForegroundColor Yellow
    dotnet dev-certs https --export-path $certFile --password "cryptic-password" --trust
} else {
    Write-Host "Development certificate already exists." -ForegroundColor Green
}

# Ensure Database directory exists for local development
$dbDir = Join-Path $projectRoot "Data"
if (-not (Test-Path -Path $dbDir)) {
    Write-Host "Creating Data directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $dbDir | Out-Null
}

Write-Host "=== Environment Setup Complete ===" -ForegroundColor Cyan
