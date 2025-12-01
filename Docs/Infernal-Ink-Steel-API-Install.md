
# 📘 Production-Grade Server Setup Guide: Infernal-Ink-Steel-Suite API

## 🔧 Prerequisites

| Requirement                     | Version/Notes                                     |
|-------------------------------|--------------------------------------------------|
| Windows Server                | 2016, 2019, or 2022                               |
| IIS (Internet Information Services) | Enabled                                       |
| .NET 8.0 Hosting Bundle       | [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime) |
| SQLite Database File          | Must exist at `C:\InfernalInkSteelSuite\infernalinksteelsuite.db` |
| Administrator Access          | Required for setup and permissions               |

---

## 1. 🔨 Publish the API Application

```bash
dotnet publish -c Release -o C:\Publish\InfernalSuiteApi
```

Copy output to:

```powershell
Copy-Item -Path "C:\Publish\InfernalSuiteApi\*" -Destination "C:\inetpub\wwwroot\InfernalSuiteApi" -Recurse -Force
```

---

## 2. ⚙️ Configure IIS

### a. Create Application Pool

- Name: `InfernalSuiteApiPool`
- .NET CLR: `No Managed Code`

### b. Create Website

- Site name: `InfernalSuiteApi`
- Physical path: `C:\inetpub\wwwroot\InfernalSuiteApi`
- Port: `5000` or `443` (distinct from Web app)
- App Pool: `InfernalSuiteApiPool`

---

## 3. ⚙️ Configuration: appsettings.json

Verify the following entries:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=C:\\InfernalInkSteelSuite\\infernalinksteelsuite.db"
},
"Jwt": {
  "Key": "YOUR_SECURE_KEY_HERE_MUST_BE_LONG_ENOUGH",
  "Issuer": "InfernalInkSteelSuite",
  "Audience": "InfernalInkSteelSuiteUsers"
}
```

- Ensure Key is strong and securely stored.
- Ensure Issuer and Audience match client expectations.

---

## 4. 🛢️ Database File Permissions

Grant Modify access for App Pool:

```powershell
$acl = Get-Acl "C:\InfernalInkSteelSuite"
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS AppPool\InfernalSuiteApiPool", "Modify", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.AddAccessRule($rule)
Set-Acl "C:\InfernalInkSteelSuite" $acl
```

Repeat for the `.db` file.

---

## 5. 📂 File Uploads (If Applicable)

If API saves uploads (e.g. `/documents`):

- Ensure uploads directory exists (e.g., `wwwroot/uploads`)
- Grant `Modify` access to `IIS AppPool\InfernalSuiteApiPool`

---

## 6. 🔐 HTTPS and CORS Configuration

### HTTPS

- Bind SSL certificate to API site on port `443`
- Required for JWT-based authentication

### CORS

- Default: Allows all origins
- For production: Restrict in `Program.cs` or `appsettings.json` to trusted domains

---

## 7. 🧪 Verification and Troubleshooting

### Swagger (if enabled)

Navigate to:

```
http://localhost:5000/swagger
```

### Common Issues

| Error            | Solution                                            |
|------------------|-----------------------------------------------------|
| HTTP 500.19      | .NET Hosting Bundle missing                         |
| DB Access Denied | Check permissions for AppPool identity              |
| JWT Rejections   | Check Key, Issuer, Audience mismatch                |

---

## ✅ Final Checklist

- [ ] API published and copied to correct location
- [ ] API App Pool created and assigned
- [ ] Database file exists and permissions granted
- [ ] appsettings.json configured with JWT + DB string
- [ ] HTTPS bound with valid cert
- [ ] Uploads folder created and writable (if needed)
- [ ] Swagger accessible (if enabled)
