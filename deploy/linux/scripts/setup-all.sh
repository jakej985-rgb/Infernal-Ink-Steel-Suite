#!/bin/bash
set -e
trap 'echo "FAIL"; exit 1' ERR

# setup-all.sh
# Purpose: Master script to run the full server setup sequence.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "============================================================"
echo "   Infernal Ink & Steel Suite - Automated Server Setup"
echo "============================================================"
echo "This script will transform this machine into a server."
echo "Steps:"
echo "  1. Base System (Updates, Security)"
echo "  2. Stack Install (Nginx, .NET 8)"
echo "  3. App Deployment (Binaries, Systemd, Config)"
echo "  4. Backup Setup (Cron, Script)"
echo "============================================================"
echo "Waiting 60 seconds for user input. Will auto-continue if no input."
if read -t 60 -p "Press [Enter] to continue or Ctrl+C to cancel..." ; then
    echo ""
    echo "User continued."
else
    echo ""
    echo "Timeout reached. Auto-continuing..."
fi

# Run scripts in order
echo ""
echo ">>> [1/4] Running 01-base-system.sh..."
bash "$SCRIPT_DIR/01-base-system.sh"

echo ""
echo ">>> [2/4] Running 02-install-stack.sh..."
bash "$SCRIPT_DIR/02-install-stack.sh"

echo ""
echo ">>> [3/4] Running 03-deploy-infernal.sh..."
bash "$SCRIPT_DIR/03-deploy-infernal.sh"

echo ""
echo ">>> [4/4] Running 04-setup-backup.sh..."
bash "$SCRIPT_DIR/04-setup-backup.sh"

echo ""
echo "============================================================"
echo "   SETUP COMPLETE!"
echo "============================================================"
echo "The server should now be running."
echo "Access the Web Portal at: http://$(hostname -I | awk '{print $1}')"
echo "Access the API at:        http://$(hostname -I | awk '{print $1}')/api/"
echo ""
echo "PASS"
