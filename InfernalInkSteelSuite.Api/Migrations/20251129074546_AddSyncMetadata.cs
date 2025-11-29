using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfernalInkSteelSuite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Documents
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Documents",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SyncId",
                table: "Documents",
                type: "TEXT",
                nullable: true); // Start nullable to allow backfill

            // Generate UUIDs for existing rows in SQLite
            // This hex manipulation attempts to create a v4 UUID string
            migrationBuilder.Sql(
                @"UPDATE Documents SET SyncId =
                    lower(hex(randomblob(4))) || '-' ||
                    lower(hex(randomblob(2))) || '-' ||
                    '4' || substr(lower(hex(randomblob(2))),2) || '-' ||
                    substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' ||
                    lower(hex(randomblob(6)))
                  WHERE SyncId IS NULL");

            // Make non-nullable
            // Note: AlterColumn in SQLite via EF Core might rebuild the table.
            // If this fails, we might need to leave it nullable or rely on EF Core's rebuild logic.
            // But EF Core MigrationBuilder.AlterColumn should handle it.
            /*
               SQLite does not support ALTER COLUMN directly. EF Core handles this by creating a new table.
               However, doing Add -> Update -> Alter in one migration might be tricky.
               For simplicity in this environment, we will leave the default as Guid.NewGuid() (random per insert in app)
               and ensure the DB has valid values.
               We will try to set the default value now.
            */

            // Re-declare as non-nullable now that we have data
            // Since we can't easily alter column in SQLite in the same step without rebuilding,
            // we will assume the initial AddColumn was enough IF we provided a default.
            // BUT we want unique defaults.
            // So we use the strategy: Add Nullable -> Sql Update -> (Ideally Alter to Non-Nullable, but we will skip enforcing constraint at DB level for now to avoid complexity, OR we rely on EF Core model definition which is Non-Nullable).

            // Actually, if the Model says Non-Nullable, EF Core expects Non-Nullable.
            // If we initially add it as Nullable in the migration, but the Model matches, it might work at runtime.
            // But let's try to be correct.
            // SQLite restriction: AddColumn can only add Nullable or Non-Nullable with Constant Default.
            // It cannot add Non-Nullable with Dynamic Default (like new uuid per row).

            // Solution: Add as Nullable (with no default or a constant default), then Update, then leave as is (technically nullable in DB schema but app treats as non-nullable).
            // OR use EF Core's "AlterColumn" to rebuild the table.

            // Let's try to Alter it to non-nullable.
             migrationBuilder.AlterColumn<Guid>(
                name: "SyncId",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"), // Default for NEW rows if app doesn't set it (which it does)
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);


            // Clients
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Clients",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SyncId",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE Clients SET SyncId =
                    lower(hex(randomblob(4))) || '-' ||
                    lower(hex(randomblob(2))) || '-' ||
                    '4' || substr(lower(hex(randomblob(2))),2) || '-' ||
                    substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' ||
                    lower(hex(randomblob(6)))
                  WHERE SyncId IS NULL");

             migrationBuilder.AlterColumn<Guid>(
                name: "SyncId",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);


            // Appointments
             migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Appointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Appointments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "Appointments",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Appointments",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SyncId",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

             migrationBuilder.Sql(
                @"UPDATE Appointments SET SyncId =
                    lower(hex(randomblob(4))) || '-' ||
                    lower(hex(randomblob(2))) || '-' ||
                    '4' || substr(lower(hex(randomblob(2))),2) || '-' ||
                    substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' ||
                    lower(hex(randomblob(6)))
                  WHERE SyncId IS NULL");

             migrationBuilder.AlterColumn<Guid>(
                name: "SyncId",
                table: "Appointments",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SyncId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "SyncId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SyncId",
                table: "Appointments");
        }
    }
}
