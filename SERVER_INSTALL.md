# Server Installation Guide: Infernal Ink & Steel Suite

This document outlines the steps to install the Infernal Ink & Steel Suite on an Ubuntu MATE (or Ubuntu Server) machine using the provided automation scripts.

## 1. Prerequisites
- **OS**: Ubuntu 22.04 LTS (Desktop or Server).
- **Network**: Internet connection (for downloading packages).
- **User**: A user with `sudo` privileges.
- **Artifacts**: You need the compiled application binaries.

## 2. Prepare Deployment Package
On your build machine, create a folder structure like this:

```text
deploy-package/
├── scripts/                # Copy from deploy/linux/scripts/
│   ├── 01-base-system.sh
│   ├── 02-install-stack.sh
│   ├── 03-deploy-infernal.sh
│   ├── 04-setup-backup.sh
│   └── setup-all.sh
├── systemd/                # Copy from deploy/linux/systemd/
│   ├── infernal-api.service
│   └── infernal-web.service
├── nginx/                  # Copy from deploy/linux/nginx/
│   └── infernal.conf
└── publish/                # YOUR COMPILED BINARIES
    ├── Api/                # Content of InfernalInkSteelSuite.Api publish output
    └── Web/                # Content of InfernalInkSteelSuite.Web publish output
```

## 3. Transfer to Server
Copy the `deploy-package` folder to the server (e.g., via USB or SCP).

```bash
# Example SCP command
scp -r deploy-package user@192.168.1.10:/home/user/
```

## 4. Run Installation
On the server, open a terminal:

```bash
cd /home/user/deploy-package/scripts
chmod +x *.sh
sudo ./setup-all.sh
```

The script will:
1.  Ask you to press Enter (or wait 60 seconds to auto-start).
2.  Update the system and install security tools (UFW, Fail2ban).
3.  Install .NET 8 and Nginx.
4.  Deploy the application and start services.
5.  Set up daily backups.

**Output**:
- Each step will output `PASS` if successful.
- If any step fails, it will output `FAIL` and stop.

## 5. Verify Installation
Visit the server's IP address in a web browser:
`http://<server-ip>/`

You should see the application running.

## 6. Maintenance
- **Backups**: Stored in `/opt/infernal-backups`.
- **Logs**: View service logs with `journalctl -u infernal-api -f`.
- **Updates**: To update the app, replace files in `/opt/infernal-publish` and restart services (`systemctl restart infernal-api infernal-web`).
