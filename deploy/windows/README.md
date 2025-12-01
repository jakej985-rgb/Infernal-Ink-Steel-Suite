# Windows Deployment Guide

## 1. Publish

Run the `publish-all.ps1` script to build the application for deployment.

```powershell
.\deploy\windows\publish-all.ps1
```

This will create the following directories:
- `C:\InfernalInkSteelSuite\publish\Api`
- `C:\InfernalInkSteelSuite\publish\Web`

## 2. Start the Applications

You can use the provided batch scripts to start the applications:

- `deploy\windows\start-api-dev.bat` (Starts API on port 5001)
- `deploy\windows\start-web-dev.bat` (Starts Web on port 5002)

## 3. Firewall Configuration

To allow other devices on your LAN to access the applications, you need to open ports 5001 and 5002 in the Windows Firewall.

Run the following PowerShell commands as Administrator:

```powershell
New-NetFirewallRule -DisplayName "Infernal API 5001" -Direction Inbound -Protocol TCP -LocalPort 5001 -Action Allow
New-NetFirewallRule -DisplayName "Infernal Web 5002" -Direction Inbound -Protocol TCP -LocalPort 5002 -Action Allow
```

## 4. Testing

Find your computer's IP address (run `ipconfig`). Then, from another device on the same network, try to access:

- API: `http://<your-ip>:5001/swagger`
- Web: `http://<your-ip>:5002/`
