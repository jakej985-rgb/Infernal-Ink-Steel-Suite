# Troubleshooting Network Access

If you cannot access the Infernal Ink & Steel Suite from another computer, follow these steps.

## 1. Check the URL
**Do NOT use ports 5001 or 5002.**
The firewall blocks these ports by default for security. You must access the application through Nginx on port 80.

- **Correct**: `http://192.168.1.10/`
- **Incorrect**: `http://192.168.1.10:5002/`

## 2. Check Service Status
Run these commands on the server to ensure everything is running:

```bash
# Check if Nginx is running (Port 80 handler)
systemctl status nginx

# Check if the App services are running
systemctl status infernal-web
systemctl status infernal-api
```

If any are "inactive" or "failed", restart them:
```bash
sudo systemctl restart nginx infernal-web infernal-api
```

## 3. Check Firewall Rules
Ensure the firewall allows traffic on port 80.

```bash
sudo ufw status verbose
```

You should see:
```text
80/tcp                     ALLOW IN    Anywhere
443/tcp                    ALLOW IN    Anywhere
```

If not, run:
```bash
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw reload
```

## 4. Check Network Bindings
Verify the apps are listening on all interfaces (`0.0.0.0`) and not just localhost (`127.0.0.1`).

```bash
sudo ss -tulpn | grep dotnet
```

You should see something like:
```text
tcp   LISTEN 0      512          0.0.0.0:5001      0.0.0.0:*    users:(("dotnet",pid=...,fd=...))
tcp   LISTEN 0      512          0.0.0.0:5002      0.0.0.0:*    users:(("dotnet",pid=...,fd=...))
```

## 5. Check Nginx Logs
If you get a "502 Bad Gateway" error, Nginx cannot talk to the .NET app. Check logs:

```bash
sudo tail -f /var/log/nginx/error.log
```
