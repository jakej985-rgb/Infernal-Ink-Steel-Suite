#!/usr/bin/env bash
set -euxo pipefail

sudo mkdir -p /opt/infernal-data /opt/infernal-uploads
sudo chown -R www-data:www-data /opt/infernal-data /opt/infernal-uploads
sudo chmod 750 /opt/infernal-data /opt/infernal-uploads
