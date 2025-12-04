#!/bin/bash
set -e
trap 'echo "FAIL"; exit 1' ERR

# 04-setup-backup.sh
# Purpose: Configure automated daily backups for the database and file uploads.

BACKUP_SCRIPT_PATH="/usr/local/bin/backup-infernal.sh"
BACKUP_DIR="/opt/infernal-backups"

echo ">>> Starting Backup Setup..."

# 1. Create Backup Directory
echo "--- Creating backup directory..."
sudo mkdir -p "$BACKUP_DIR"
sudo chown root:root "$BACKUP_DIR"
sudo chmod 700 "$BACKUP_DIR"

# 2. Create the Backup Script
echo "--- Creating backup script at $BACKUP_SCRIPT_PATH..."
cat <<EOF | sudo tee "$BACKUP_SCRIPT_PATH" > /dev/null
#!/bin/bash
# Auto-generated backup script for Infernal Ink & Steel Suite
set -e
trap 'echo "FAIL"; exit 1' ERR

BACKUP_ROOT="$BACKUP_DIR"
TIMESTAMP=\$(date +%Y%m%d-%H%M%S)
ARCHIVE_NAME="infernal-backup-\$TIMESTAMP.tar.gz"

mkdir -p "\$BACKUP_ROOT"

echo "Starting backup: \$ARCHIVE_NAME"
tar -czf "\$BACKUP_ROOT/\$ARCHIVE_NAME" -C /opt infernal-data infernal-uploads

echo "Backup successful: \$BACKUP_ROOT/\$ARCHIVE_NAME"
find "\$BACKUP_ROOT" -name "infernal-backup-*.tar.gz" -mtime +30 -delete
echo "PASS"
EOF

sudo chmod +x "$BACKUP_SCRIPT_PATH"

# 3. Setup Cron Job
echo "--- Setting up Cron Job (Daily at 3 AM)..."
CRON_FILE="/etc/cron.d/infernal-backup"

cat <<EOF | sudo tee "$CRON_FILE" > /dev/null
# Run Infernal Ink & Steel backup daily at 3:00 AM
0 3 * * * root $BACKUP_SCRIPT_PATH >> /var/log/infernal-backup.log 2>&1
EOF

sudo chmod 644 "$CRON_FILE"

echo ">>> Backup Setup Complete."
echo "PASS"
