#!/bin/bash
set -e
trap 'echo "FAIL"; exit 1' ERR

# 03-deploy-infernal.sh
# Purpose: Deploy the Infernal Ink & Steel Suite application (API and Web) and configure services.

# Paths
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DEPLOY_ROOT="$(dirname "$SCRIPT_DIR")" # deploy/linux
PUBLISH_SRC="$DEPLOY_ROOT/publish"     # Expecting deploy/linux/publish/Api and .../Web
DEST_ROOT="/opt/infernal-publish"
DATA_ROOT="/opt/infernal-data"
UPLOAD_ROOT="/opt/infernal-uploads"

echo ">>> Starting Application Deployment..."

# 1. Check for Publish Artifacts
echo "--- Checking for application binaries..."
if [ ! -d "$PUBLISH_SRC/Api" ] || [ ! -d "$PUBLISH_SRC/Web" ]; then
    echo "ERROR: Publish artifacts not found!"
    echo "Expected locations:"
    echo "  - $PUBLISH_SRC/Api"
    echo "  - $PUBLISH_SRC/Web"
    echo "FAIL"
    exit 1
fi

# 2. Create Directories & Set Permissions
echo "--- Creating directories..."
sudo mkdir -p "$DEST_ROOT/Api"
sudo mkdir -p "$DEST_ROOT/Web"
sudo mkdir -p "$DATA_ROOT"
sudo mkdir -p "$UPLOAD_ROOT"

echo "--- Setting permissions..."
sudo chown -R www-data:www-data "$DEST_ROOT"
sudo chown -R www-data:www-data "$DATA_ROOT"
sudo chown -R www-data:www-data "$UPLOAD_ROOT"
sudo chmod 750 "$DATA_ROOT"
sudo chmod 750 "$UPLOAD_ROOT"

# 3. Copy Application Files
echo "--- Copying application files..."
sudo systemctl stop infernal-api infernal-web || true

sudo cp -r "$PUBLISH_SRC/Api/." "$DEST_ROOT/Api/"
sudo cp -r "$PUBLISH_SRC/Web/." "$DEST_ROOT/Web/"

sudo chmod +x "$DEST_ROOT/Api/InfernalInkSteelSuite.Api.dll" || true
sudo chmod +x "$DEST_ROOT/Web/InfernalInkSteelSuite.Web.dll" || true
sudo chown -R www-data:www-data "$DEST_ROOT"

# 4. Install Systemd Services
echo "--- Installing Systemd Services..."
sudo cp "$DEPLOY_ROOT/systemd/infernal-api.service" /etc/systemd/system/
sudo cp "$DEPLOY_ROOT/systemd/infernal-web.service" /etc/systemd/system/

sudo systemctl daemon-reload
sudo systemctl enable infernal-api
sudo systemctl enable infernal-web
sudo systemctl start infernal-api
sudo systemctl start infernal-web

# 5. Configure Nginx
echo "--- Configuring Nginx..."
sudo cp "$DEPLOY_ROOT/nginx/infernal.conf" /etc/nginx/sites-available/infernal.conf

if [ ! -f /etc/nginx/sites-enabled/infernal.conf ]; then
    sudo ln -s /etc/nginx/sites-available/infernal.conf /etc/nginx/sites-enabled/
fi

if [ -f /etc/nginx/sites-enabled/default ]; then
    sudo rm /etc/nginx/sites-enabled/default
fi

sudo nginx -t
sudo systemctl reload nginx

echo ">>> Application Deployment Complete."
echo "PASS"
