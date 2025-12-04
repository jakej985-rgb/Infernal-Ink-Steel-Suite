#!/bin/bash
set -e
trap 'echo "FAIL"; exit 1' ERR

# 02-install-stack.sh
# Purpose: Install the required technology stack (.NET 8 and Nginx).

echo ">>> Starting Stack Installation..."

# 1. Install .NET 8 Runtime
echo "--- Installing .NET 8 Runtime..."
if ! command -v dotnet &> /dev/null; then
    wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    sudo apt-get update
    sudo apt-get install -y dotnet-runtime-8.0 aspnetcore-runtime-8.0
else
    echo "Dotnet is already installed."
fi
dotnet --info

# 2. Install Nginx
echo "--- Installing Nginx..."
sudo apt-get install -y nginx
sudo systemctl enable nginx
sudo systemctl start nginx
nginx -v

echo ">>> Stack Installation Complete."
echo "PASS"
