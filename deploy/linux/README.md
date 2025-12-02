# Linux Deployment

## Prerequisites

- .NET 8 SDK
- `systemd` (for service management)
- `sudo` privileges

## Scripts

All scripts show **PASS/FAIL** indicators. You can force them to wait for a key press by setting `INFERNAL_PAUSE=1`.

### `publish-all.sh`

Publishes both projects to `/opt/infernal-publish`.

**Usage:**
```bash
./publish-all.sh
# Or with pause:
INFERNAL_PAUSE=1 ./publish-all.sh
```

### `setup-dirs.sh`

Creates necessary directories and sets permissions.

**Usage:**
```bash
./setup-dirs.sh
```

### `systemd/install-services.sh`

Installs and starts the systemd services.

**Usage:**
```bash
cd systemd
./install-services.sh
```
