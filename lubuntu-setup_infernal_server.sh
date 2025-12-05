#!/usr/bin/env bash
# setup_infernal_server.sh
# Turn a fresh Lubuntu install into a server for Infernal Ink & Steel Suite

set -e

# --- Helpers ---
require_root() {
  if [ "$EUID" -ne 0 ]; then
    echo "Please run this script as root, e.g.: sudo ./setup_infernal_server.sh"
    exit 1
  fi
}

info()  { echo -e "\n[INFO] $*"; }
warn()  { echo -e "\n[WARN] $*"; }
error() { echo -e "\n[ERROR] $*"; }

require_root

# --- Basic system update ---
info "Updating package lists and upgrading system..."
apt update
apt -y upgrade

# --- Install base server tools ---
info "Installing base server tools (SSH, firewall, nginx, utilities)..."
apt -y install \
  openssh-server \
  ufw \
  nginx \
  curl \
  wget \
  unzip \
  git \
  gpg

# --- Configure UFW firewall ---
info "Configuring UFW firewall..."
ufw allow OpenSSH
ufw allow "Nginx Full" || true
ufw --force enable

# --- Install Microsoft .NET 8 SDK + runtime ---
info "Setting up Microsoft package repository for .NET..."

# Determine Ubuntu codename (jammy, noble, etc.)
CODENAME=$(lsb_release -cs)

wget https://packages.microsoft.com/config/ubuntu/$CODENAME/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb
dpkg -i /tmp/packages-microsoft-prod.deb
rm /tmp/packages-microsoft-prod.deb

apt update

info "Installing .NET 8 SDK + ASP.NET Core runtime..."
apt -y install dotnet-sdk-8.0 aspnetcore-runtime-8.0

# --- Install GitHub Desktop (ShiftKey Linux build) ---
info "Adding GitHub Desktop repository (ShiftKey) and installing..."

wget -qO - https://packagecloud.io/shiftkey/desktop/gpgkey \
  | gpg --dearmor -o /usr/share/keyrings/shiftkey-desktop.gpg

echo "deb [signed-by=/usr/share/keyrings/shiftkey-desktop.gpg] https://packagecloud.io/shiftkey/desktop/any any main" \
  > /etc/apt/sources.list.d/shiftkey-desktop.list

apt update
apt -y install github-desktop || warn "GitHub Desktop install failed – check errors above."

# --- Create folders for your app ---
info "Creating folder structure under /srv/InfernalInkSteelSuite..."
BASE_DIR="/srv/InfernalInkSteelSuite"
API_DIR="$BASE_DIR/Api"
WEB_DIR="$BASE_DIR/Web"

mkdir -p "$API_DIR"
mkdir -p "$WEB_DIR"

# Make current user owner (assumes you ran with sudo from your main user)
if [ -n "$SUDO_USER" ]; then
  chown -R "$SUDO_USER":"$SUDO_USER" "$BASE_DIR"
fi

info "Folders ready:"
echo "  API: $API_DIR"
echo "  WEB: $WEB_DIR"
echo "Copy your published or built API/Web files into these directories after this script finishes."

# --- Create systemd services ---
info "Creating systemd service for Infernal Ink API (port 5000)..."

cat >/etc/systemd/system/infernal-api.service <<EOF
[Unit]
Description=Infernal Ink & Steel API
After=network.target

[Service]
WorkingDirectory=$API_DIR
ExecStart=/usr/bin/dotnet $API_DIR/InfernalInkSteelSuite.Api.dll
Restart=always
RestartSec=5
SyslogIdentifier=infernal-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
EOF

info "Creating systemd service for Infernal Ink Web (port 5001)..."

cat >/etc/systemd/system/infernal-web.service <<EOF
[Unit]
Description=Infernal Ink & Steel Web
After=network.target infernal-api.service

[Service]
WorkingDirectory=$WEB_DIR
ExecStart=/usr/bin/dotnet $WEB_DIR/InfernalInkSteelSuite.Web.dll
Restart=always
RestartSec=5
SyslogIdentifier=infernal-web
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5001

[Install]
WantedBy=multi-user.target
EOF

# --- Reload systemd and enable services ---
info "Reloading systemd daemon..."
systemctl daemon-reload

info "Enabling services to start on boot..."
systemctl enable infernal-api.service
systemctl enable infernal-web.service

# --- Nginx reverse proxy config ---
info "Configuring Nginx reverse proxy..."

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

info "Testing Nginx configuration..."
nginx -t

info "Restarting Nginx..."
systemctl restart nginx

# --- (Optional) Make GUI start/stop #easier for 'server mode' ---
#warn "If you want Lubuntu to boot to #text-only server mode, run this #later:"
#echo "  sudo systemctl set-default #multi-user.target"
#warn "To go back to GUI login on #boot:"
#echo "  sudo systemctl set-default #graphical.target"

info "All done!"
echo "Next steps:"
echo "  1) Copy or build your API into: $API_DIR"
echo "     (must contain InfernalInkSteelSuite.Api.dll)"
echo "  2) Copy or build your Web into: $WEB_DIR"
echo "     (must contain InfernalInkSteelSuite.Web.dll)"
echo "  3) Start the services:"
echo "       sudo systemctl start infernal-api"
echo "       sudo systemctl start infernal-web"
echo "  4) Then visit:  http://YOUR-SERVER-IP/  from another device."
echo "  5) Launch GitHub Desktop from the Lubuntu menu to work with your repo."