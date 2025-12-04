#!/usr/bin/env bash
set -euxo pipefail

echo "=== Infernal Ink & Steel - Publish All ==="

# Default exit code is 0; 'set -e' will abort on first error

cleanup() {
  local code=$?
  if [ $code -eq 0 ]; then
    echo "✅ SUCCESS: Published API and Web to $OUTPUT_ROOT"
  else
    echo "❌ FAILED: Publishing failed (exit code $code)"
  fi

  # Optional interactive pause: only if INFERNAL_PAUSE is set
  if [ "${INFERNAL_PAUSE:-0}" != "0" ]; then
    echo
    read -n1 -r -p "Press any key to close..." key
    echo
  fi

  exit $code
}
trap cleanup EXIT

CONFIG=${1:-Release}
OUTPUT_ROOT=${2:-/opt/infernal-publish}

mkdir -p "$OUTPUT_ROOT"

# API
dotnet publish ./InfernalInkSteelSuite.Api/InfernalInkSteelSuite.Api.csproj \
  -c "$CONFIG" -o "$OUTPUT_ROOT/Api" --self-contained false

# WEB
dotnet publish ./InfernalInkSteelSuite.Web/InfernalInkSteelSuite.Web.csproj \
  -c "$CONFIG" -o "$OUTPUT_ROOT/Web" --self-contained false
