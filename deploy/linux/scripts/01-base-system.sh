#!/bin/bash
set -e
trap 'echo "FAIL"; exit 1' ERR

# 01-base-system.sh
# Purpose: Prepare the Ubuntu system with updates, security tools, and basic configuration.

echo ">>> Starting Base System Setup..."

# 1. Update and Upgrade System
echo "--- Updating package lists and upgrading system..."
export DEBIAN_FRONTEND=noninteractive
sudo apt-get update
sudo apt-get upgrade -y

# 2. Install Essential Tools & Security Packages
echo "--- Installing essential tools and security packages..."
sudo apt-get install -y curl wget git unzip ufw fail2ban unattended-upgrades openssh-server

# 3. Configure Firewall (UFW)
echo "--- Configuring UFW Firewall..."
# Reset to default to ensure clean state
sudo ufw --force reset
# Default policies
sudo ufw default deny incoming
sudo ufw default allow outgoing
# Allow SSH (Port 22) - CRITICAL to do this before enabling
sudo ufw allow 22/tcp
# Allow HTTP/HTTPS
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
# Enable UFW
echo "y" | sudo ufw enable
sudo ufw status verbose

# 4. Configure Fail2ban
echo "--- Configuring Fail2ban..."
# Ensure it's running and enabled
sudo systemctl enable fail2ban
sudo systemctl start fail2ban

# 5. Configure Unattended Upgrades
echo "--- Enabling Unattended Upgrades..."
sudo dpkg-reconfigure -f noninteractive unattended-upgrades

echo ">>> Base System Setup Complete."
echo "PASS"
