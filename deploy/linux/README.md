# Linux Deployment Guide

## 1. Prerequisites

- Install .NET 8 SDK/Runtime.
- Ensure you have `sudo` privileges.

## 2. Publish

Run the `publish-all.sh` script to build the application for deployment.

```bash
chmod +x deploy/linux/publish-all.sh
./deploy/linux/publish-all.sh
```

This will create the following directories in `/opt/infernal-publish` (or your specified output):
- `/opt/infernal-publish/Api`
- `/opt/infernal-publish/Web`

## 3. Setup Directories

Run the setup script to create data directories and set permissions.

```bash
chmod +x deploy/linux/setup-dirs.sh
./deploy/linux/setup-dirs.sh
```

## 4. Install Services

Copy the systemd service files and start the services.

```bash
cd deploy/linux/systemd
chmod +x install-services.sh
./install-services.sh
```

## 5. Optional: Nginx Reverse Proxy

If you want to use Nginx as a reverse proxy:

```bash
sudo cp deploy/linux/nginx/infernal.conf /etc/nginx/sites-available/
sudo ln -s /etc/nginx/sites-available/infernal.conf /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

## 6. Testing

- Check service status: `systemctl status infernal-api infernal-web`
- Test API: `curl http://<server-ip>:5001/health`
- Test Web: Open `http://<server-ip>:5002/` in your browser.
