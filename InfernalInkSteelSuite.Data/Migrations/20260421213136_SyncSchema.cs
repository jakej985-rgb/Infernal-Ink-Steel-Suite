using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfernalInkSteelSuite.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Users",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SyncId",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "HasSyncConflict",
                table: "Appointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncConflictNotes",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SyncId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    ArtistId = table.Column<int>(type: "INTEGER", nullable: false),
                    Placement = table.Column<string>(type: "TEXT", nullable: false),
                    Style = table.Column<string>(type: "TEXT", nullable: false),
                    IsCoverUp = table.Column<bool>(type: "INTEGER", nullable: false),
                    Width = table.Column<double>(type: "REAL", nullable: false),
                    Height = table.Column<double>(type: "REAL", nullable: false),
                    CoverageLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    LineComplexity = table.Column<int>(type: "INTEGER", nullable: false),
                    ShadingComplexity = table.Column<int>(type: "INTEGER", nullable: false),
                    ColorComplexity = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedHoursLow = table.Column<double>(type: "REAL", nullable: false),
                    EstimatedHoursHigh = table.Column<double>(type: "REAL", nullable: false),
                    PriceLow = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceHigh = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShopMinimum = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecommendedDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfidenceScore = table.Column<double>(type: "REAL", nullable: false),
                    SimilarJobsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    PhotoPath = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShopName = table.Column<string>(type: "TEXT", nullable: false),
                    LogoPath = table.Column<string>(type: "TEXT", nullable: false),
                    AccentColor = table.Column<string>(type: "TEXT", nullable: false),
                    SidebarArtworkPath = table.Column<string>(type: "TEXT", nullable: false),
                    LoginHeadline = table.Column<string>(type: "TEXT", nullable: false),
                    SpecialMessageText = table.Column<string>(type: "TEXT", nullable: false),
                    LoginBackgroundPath = table.Column<string>(type: "TEXT", nullable: false),
                    LoginHeadlineFontFamily = table.Column<string>(type: "TEXT", nullable: false),
                    LoginTaglineFontFamily = table.Column<string>(type: "TEXT", nullable: false),
                    LoginTextColor = table.Column<string>(type: "TEXT", nullable: false),
                    TattooPerHour = table.Column<double>(type: "decimal(18,2)", nullable: false),
                    PiercingSingle = table.Column<double>(type: "decimal(18,2)", nullable: false),
                    PiercingMulti = table.Column<double>(type: "REAL", nullable: false),
                    ShopMinimumRate = table.Column<double>(type: "decimal(18,2)", nullable: false),
                    EnableAutomaticHolidayThemes = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSpecialMessageEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShopHoursJson = table.Column<string>(type: "TEXT", nullable: false),
                    TaxRate = table.Column<double>(type: "REAL", nullable: false),
                    DepositType = table.Column<string>(type: "TEXT", nullable: false),
                    DepositAmount = table.Column<double>(type: "REAL", nullable: false),
                    BookingBufferMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    CancellationPolicy = table.Column<string>(type: "TEXT", nullable: false),
                    AppointmentDurationPresetsJson = table.Column<string>(type: "TEXT", nullable: false),
                    SpecialHoursJson = table.Column<string>(type: "TEXT", nullable: false),
                    NotificationSettingsJson = table.Column<string>(type: "TEXT", nullable: false),
                    BackupSettingsJson = table.Column<string>(type: "TEXT", nullable: false),
                    LinkedAccountsJson = table.Column<string>(type: "TEXT", nullable: false),
                    AppFontSize = table.Column<double>(type: "REAL", nullable: false),
                    LastSyncUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "ShopSettings");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SyncId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "HasSyncConflict",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SyncConflictNotes",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
