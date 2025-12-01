#!/usr/bin/env bash
set -euxo pipefail

sudo cp infernal-api.service /etc/systemd/system/
sudo cp infernal-web.service /etc/systemd/system/

sudo systemctl daemon-reload
sudo systemctl enable infernal-api infernal-web
sudo systemctl start infernal-api infernal-web
