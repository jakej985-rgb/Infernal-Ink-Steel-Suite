#!/usr/bin/env bash
set -euxo pipefail

CONFIG=${1:-Release}
OUTPUT_ROOT=${2:-/opt/infernal-publish}

mkdir -p "$OUTPUT_ROOT"

# API
dotnet publish ./InfernalInkSteelSuite.Api/InfernalInkSteelSuite.Api.csproj \
  -c "$CONFIG" -o "$OUTPUT_ROOT/Api" --self-contained false

# WEB
dotnet publish ./InfernalInkSteelSuite.Web/InfernalInkSteelSuite.Web.csproj \
  -c "$CONFIG" -o "$OUTPUT_ROOT/Web" --self-contained false
