#!/usr/bin/env bash
set -euxo pipefail

echo "=== Infernal Ink & Steel - Setup Dirs ==="

cleanup() {
  local code=$?
  if [ $code -eq 0 ]; then
    echo "✅ SUCCESS: Directories created and permissions set"
  else
    echo "❌ FAILED: Setup failed (exit code $code)"
  fi

  if [ "${INFERNAL_PAUSE:-0}" != "0" ]; then
    echo
    read -n1 -r -p "Press any key to close..." key
    echo
  fi

  exit $code
}
trap cleanup EXIT

sudo mkdir -p /opt/infernal-data /opt/infernal-uploads
sudo chown -R www-data:www-data /opt/infernal-data /opt/infernal-uploads
sudo chmod 750 /opt/infernal-data /opt/infernal-uploads
