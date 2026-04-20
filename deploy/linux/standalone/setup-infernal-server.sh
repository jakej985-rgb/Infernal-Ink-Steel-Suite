#!/usr/bin/env bash
set -euo pipefail

echo "=== Infernal Ink & Steel - Xubuntu Auto Setup ==="

# ----- CONFIG -----
# Default install paths
BASE_DIR="/InfernalInkSteelSuite"
API_DIR="$BASE_DIR/publish/Api"
WEB_DIR="$BASE_DIR/publish/Web"
DATA_DIR="$BASE_DIR/Data"
UPLOADS_DIR="$BASE_DIR/Uploads"

# ----- ASK FOR USER & BACKUP -----
if [[ $EUID -ne 0 ]]; then
  echo "Please run this script with sudo:"
  echo "  sudo ./setup-infernal-server.sh"
  exit 1
fi

read -rp "Enter the Linux username that should own the app files (e.g. your login user): " APPUSER
if [[ -z "$APPUSER" ]]; then
  echo "Username cannot be empty. Aborting."
  exit 1
fi

echo
echo "If you have a backup tar (e.g. IIS-Server-Backup.tar.gz), enter the full path."
echo "Example: /media/$APPUSER/USB/IIS-Server-Backup.tar.gz"
read -rp "Backup tar path (or leave blank to skip restore): " BACKUP_TAR

# ----- UPDATE SYSTEM & INSTALL BASICS -----
echo
echo "=== Updating system & installing basic tools... ==="
apt update
apt upgrade -y
apt install -y git curl wget htop openssh-server ufw apt-transport-https gpg

systemctl enable ssh
systemctl start ssh

# ----- INSTALL DOTNET 8 -----
if ! command -v dotnet >/dev/null 2>&1; then
  echo
  echo "=== Installing .NET 8 SDK + ASP.NET runtime... ==="
  wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb
  dpkg -i /tmp/packages-microsoft-prod.deb
  rm /tmp/packages-microsoft-prod.deb
  apt update
  apt install -y dotnet-sdk-8.0 aspnetcore-runtime-8.0
else
  echo
  echo "dotnet already installed, skipping .NET installation."
fi

echo
dotnet --info || echo "Warning: dotnet --info failed. Check .NET install if you see errors later."

# ----- CREATE FOLDERS -----
echo
echo "=== Creating directory structure under $BASE_DIR ==="
mkdir -p "$API_DIR" "$WEB_DIR" "$DATA_DIR" "$UPLOADS_DIR"
chown -R "$APPUSER:$APPUSER" "$BASE_DIR"

# ----- RESTORE BACKUP (IF PROVIDED) -----
if [[ -n "$BACKUP_TAR" ]]; then
  if [[ -f "$BACKUP_TAR" ]]; then
    echo
    echo "=== Restoring from backup: $BACKUP_TAR ==="
    # Extract to root so it recreates /InfernalInkSteelSuite exactly as it was
    tar -xzvf "$BACKUP_TAR" -C /
    chown -R "$APPUSER:$APPUSER" "$BASE_DIR"
  else
    echo "Backup tar not found at: $BACKUP_TAR"
    echo "Continuing WITHOUT restore. You can copy files manually later."
  fi
else
  echo
  echo "No backup tar path provided. Skipping restore."
  echo "Make sure you manually copy your publish folders, DB, and Uploads into:"
  echo "  $API_DIR"
  echo "  $WEB_DIR"
  echo "  $DATA_DIR"
  echo "  $UPLOADS_DIR"
fi

# ----- CREATE DEFAULT APPSETTINGS IF MISSING -----
APPSETTINGS="$API_DIR/appsettings.json"
if [[ ! -f "$APPSETTINGS" ]]; then
  echo
  echo "appsettings.json not found in $API_DIR, creating a default one..."

  # Generate a unique JWT secret if not provided
  JWT_SECRET=$(LC_ALL=C tr -dc 'A-Za-z0-9' < /dev/urandom | head -c 32)
  
  cat > "$APPSETTINGS" <<EOF
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=$DATA_DIR/infernalinksteel.db"
  },
  "FileStorage": {
    "RootPath": "$UPLOADS_DIR"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "$JWT_SECRET",
    "Issuer": "InfernalInkSteelSuite.Api",
    "Audience": "InfernalInkSteelSuite.Clients",
    "ExpiryHours": 8
  }
}
EOF
  echo "  [OK] Default appsettings.json created with generated JWT secret."
  chown "$APPUSER:$APPUSER" "$APPSETTINGS"
else
  echo
  echo "Found existing appsettings.json at $APPSETTINGS"
  echo "Make sure the paths inside point to:"
  echo "  Data Source=$DATA_DIR/infernalinksteel.db"
  echo "  RootPath=$UPLOADS_DIR"
fi

# ----- CREATE SYSTEMD SERVICE: API -----
echo
echo "=== Creating systemd service for API (infernal-api.service) ==="
cat > /etc/systemd/system/infernal-api.service <<EOF
[Unit]
Description=Infernal Ink & Steel API
After=network.target

[Service]
WorkingDirectory=$API_DIR
ExecStart=/usr/bin/dotnet $API_DIR/InfernalInkSteelSuite.Api.dll
Restart=always
RestartSec=10
User=$APPUSER
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5000

[Install]
WantedBy=multi-user.target
EOF

# ----- CREATE SYSTEMD SERVICE: WEB -----
echo
echo "=== Creating systemd service for Web (infernal-web.service) ==="
cat > /etc/systemd/system/infernal-web.service <<EOF
[Unit]
Description=Infernal Ink & Steel Web
After=network.target infernal-api.service

[Service]
WorkingDirectory=$WEB_DIR
ExecStart=/usr/bin/dotnet $WEB_DIR/InfernalInkSteelSuite.Web.dll
Restart=always
RestartSec=10
User=$APPUSER
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://127.0.0.1:5001

[Install]
WantedBy=multi-user.target
EOF

# ----- RELOAD & ENABLE SERVICES -----
echo
echo "=== Enabling and starting services... ==="
systemctl daemon-reload
systemctl enable infernal-api.service
systemctl enable infernal-web.service
systemctl start infernal-api.service || echo "Warning: infernal-api failed to start"
echo "Waiting for API to initialize database..."
sleep 5
systemctl start infernal-web.service || echo "Warning: infernal-web failed to start"

systemctl status infernal-api.service --no-pager || true
systemctl status infernal-web.service --no-pager || true

# ----- INSTALL NGINX & CONFIGURE PROXY -----
echo
echo "=== Installing and configuring Nginx reverse proxy... ==="
apt install -y nginx
systemctl enable nginx
systemctl start nginx

NGINX_SITE="/etc/nginx/sites-available/infernal-ink"
cat >"$NGINX_SITE" <<'EOF'
server {
    listen 80;
    listen [::]:80;
    server_name _;

    # Proxy /api -> API service on port 5000
    location /api/ {
        proxy_pass         http://127.0.0.1:5000/;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }

    # Everything else -> Web app on port 5001
    location / {
        proxy_pass         http://127.0.0.1:5001/;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
EOF

ln -sf "$NGINX_SITE" /etc/nginx/sites-enabled/infernal-ink
rm -f /etc/nginx/sites-enabled/default || true
systemctl restart nginx

# ----- FIREWALL -----
echo
echo "=== Configuring UFW firewall (allowing SSH and Port 80/443)... ==="
ufw allow OpenSSH
ufw allow 80/tcp
ufw allow 443/tcp
# Deny direct access to app ports from external network
ufw deny 5000/tcp
ufw deny 5001/tcp
yes | ufw enable || true
ufw status

echo
echo "=== Setup complete! ==="
echo "Check your services with:"
echo "  systemctl status infernal-api.service"
echo "  systemctl status infernal-web.service"
echo "  systemctl status nginx"
echo
echo "From another device on your network, you should be able to visit:"
echo "  http://<server-ip>/       (Portal)"
echo "  http://<server-ip>/api/   (API Health/Health)"
echo
echo "If something fails to start, run:"
echo "  journalctl -u infernal-api.service -n 100 --no-pager"
echo "  journalctl -u infernal-web.service -n 100 --no-pager"
echo "==============================="
