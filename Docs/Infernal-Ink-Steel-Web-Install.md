
# 📘 Production-Grade Server Setup Guide: Infernal-Ink-Steel-Suite Web

## 🔧 Prerequisites

| Requirement                     | Version/Notes                                     |
|-------------------------------|--------------------------------------------------|
| Windows Server                | 2016, 2019, or 2022 (GUI mode recommended)       |
| IIS (Internet Information Services) | Enabled via Server Manager / DISM              |
| .NET 8.0 Hosting Bundle       | [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime) |
| Administrator Access          | For file deployment, service configuration, and permissions |
| SQLite                        | No installation needed, just file access         |

## 1. 🔨 Build and Publish the Application

```bash
dotnet publish -c Release -o C:\Publish\InfernalSuiteWeb
```

## 2. 📂 Deploy to IIS Directory

```powershell
New-Item -ItemType Directory -Path "C:\inetpub\wwwroot\InfernalSuiteWeb" -Force
Copy-Item -Path "C:\Publish\InfernalSuiteWeb\*" -Destination "C:\inetpub\wwwroot\InfernalSuiteWeb" -Recurse -Force
```

## 3. ⚙️ Configure IIS

### a. Create Application Pool

- Name: `InfernalSuitePool`
- .NET CLR Version: `No Managed Code`

### b. Set Advanced Pool Options

- `Start Mode`: **AlwaysRunning**
- `Idle Time-out`: **0**
- `Enable 32-Bit Applications`: **False**
- `Process Model > Identity`: `ApplicationPoolIdentity`

### c. Create Website

- Site name: `InfernalSuiteWeb`
- Path: `C:\inetpub\wwwroot\InfernalSuiteWeb`
- Port: `80` or `443` for HTTPS
- Use `InfernalSuitePool`

## 4. 🛢️ SQLite Database Setup

- Location: `C:\InfernalInkSteelSuite\infernalinksteelsuite.db`
- Permissions for `IIS AppPool\InfernalSuitePool` on folder and file.

## 5. 📎 File Uploads Configuration

```powershell
New-Item -ItemType Directory -Path "C:\inetpub\wwwroot\InfernalSuiteWeb\wwwroot\uploads" -Force
# Set Modify permissions for IIS AppPool\InfernalSuitePool
```

## 6. 🔐 HTTPS Setup

- Obtain certificate
- Bind to site in IIS
- Optional: Redirect HTTP to HTTPS using URL Rewrite

## 7. 🔍 Logging and Troubleshooting

### Enable stdout logging in `web.config`:

```xml
<aspNetCore stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" />
```

Create a `logs` folder and grant Modify access.

## 8. 🔒 IIS Security Hardening Tips

| Setting                  | Recommendation                             |
|--------------------------|--------------------------------------------|
| Request Filtering        | Block hidden segments                      |
| Directory Browsing       | Disabled                                   |
| Tracing                  | Disabled unless debugging                  |
| Custom Errors            | Friendly errors                            |
| Caching                  | Enable static file caching                 |

## 9. 🧪 Auto-Restart on Failure

Ensure IIS pool has:

- **Rapid-Fail Protection** enabled
- Recycling triggers set
- `web.config` includes environment vars

## ✅ Final Checklist

- [ ] .NET Hosting Bundle installed
- [ ] Files deployed
- [ ] App pool created and permissions set
- [ ] Database in place with correct permissions
- [ ] HTTPS configured (optional)
- [ ] Logging enabled and folder created
- [ ] Uploads folder with write access
- [ ] Accessible via browser
