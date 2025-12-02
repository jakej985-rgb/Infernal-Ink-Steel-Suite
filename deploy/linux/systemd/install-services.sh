#!/usr/bin/env bash
set -euxo pipefail

echo "=== Infernal Ink & Steel - Install Services ==="

cleanup() {
  local code=$?
  if [ $code -eq 0 ]; then
    echo "✅ SUCCESS: Services installed and started"
  else
    echo "❌ FAILED: Service installation failed (exit code $code)"
  fi

  if [ "${INFERNAL_PAUSE:-0}" != "0" ]; then
    echo
    read -n1 -r -p "Press any key to close..." key
    echo
  fi

  exit $code
}
trap cleanup EXIT

sudo cp infernal-api.service /etc/systemd/system/
sudo cp infernal-web.service /etc/systemd/system/

sudo systemctl daemon-reload
sudo systemctl enable infernal-api infernal-web
sudo systemctl start infernal-api infernal-web
