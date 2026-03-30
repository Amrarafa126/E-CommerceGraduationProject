using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class A1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "companies",
                newName: "IsActive");

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "productImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "companies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "productImages");

            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "companies");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "companies",
                newName: "IsVerified");
        }
    }
}
