# Windows Deployment

## Prerequisites

- .NET 8 SDK
- PowerShell 5.1 or newer (default on Windows 10/11)

## Scripts

All scripts now show **PASS/FAIL** indicators and will wait for a key press before closing when run interactively.

### `publish-all.bat` / `publish-all.ps1`

Publishes both the API and Web projects to `C:\InfernalInkSteelSuite\publish`.

**Usage:**
Double-click `publish-all.bat` or run in PowerShell:
```powershell
.\publish-all.ps1
```

### `start-api-dev.bat` / `start-api-dev.ps1`

Starts the API in development mode.

**Usage:**
Double-click `start-api-dev.bat` or run in PowerShell:
```powershell
.\start-api-dev.ps1
```

### `start-web-dev.bat` / `start-web-dev.ps1`

Starts the Web app in development mode.

**Usage:**
Double-click `start-web-dev.bat` or run in PowerShell:
```powershell
.\start-web-dev.ps1
```
