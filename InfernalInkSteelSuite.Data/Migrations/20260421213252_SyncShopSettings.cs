using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfernalInkSteelSuite.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncShopSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShopSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ShopSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "ShopSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ShopSettings",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SyncId",
                table: "ShopSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShopSettings");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ShopSettings");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "ShopSettings");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ShopSettings");

            migrationBuilder.DropColumn(
                name: "SyncId",
                table: "ShopSettings");
        }
    }
}
