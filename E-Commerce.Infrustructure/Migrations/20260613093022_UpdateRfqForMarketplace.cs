using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRfqForMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SellerCompanyId",
                table: "RfqRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "RfqRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationCity",
                table: "RfqRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationCountry",
                table: "RfqRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "RfqRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerms",
                table: "RfqRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredShippingMethod",
                table: "RfqRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredCertifications",
                table: "RfqRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierRequirements",
                table: "RfqRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "RfqRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Piece");

            migrationBuilder.AddColumn<int>(
                name: "LeadTimeDays",
                table: "RfqQuotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SampleAvailable",
                table: "RfqQuotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SellerCompanyId",
                table: "RfqQuotes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RfqRequests_CategoryId",
                table: "RfqRequests",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RfqRequests_IsPublic",
                table: "RfqRequests",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_RfqQuotes_SellerCompanyId",
                table: "RfqQuotes",
                column: "SellerCompanyId");

            migrationBuilder.Sql(@"
                UPDATE q
                SET q.SellerCompanyId = r.SellerCompanyId
                FROM RfqQuotes q
                INNER JOIN RfqRequests r ON q.RfqRequestId = r.Id
                WHERE r.SellerCompanyId IS NOT NULL
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_RfqQuotes_Companies_SellerCompanyId",
                table: "RfqQuotes",
                column: "SellerCompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RfqRequests_Categories_CategoryId",
                table: "RfqRequests",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RfqQuotes_Companies_SellerCompanyId",
                table: "RfqQuotes");

            migrationBuilder.DropForeignKey(
                name: "FK_RfqRequests_Categories_CategoryId",
                table: "RfqRequests");

            migrationBuilder.DropIndex(
                name: "IX_RfqRequests_CategoryId",
                table: "RfqRequests");

            migrationBuilder.DropIndex(
                name: "IX_RfqRequests_IsPublic",
                table: "RfqRequests");

            migrationBuilder.DropIndex(
                name: "IX_RfqQuotes_SellerCompanyId",
                table: "RfqQuotes");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "DestinationCity",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "DestinationCountry",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "PreferredShippingMethod",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "RequiredCertifications",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "SupplierRequirements",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "RfqRequests");

            migrationBuilder.DropColumn(
                name: "LeadTimeDays",
                table: "RfqQuotes");

            migrationBuilder.DropColumn(
                name: "SampleAvailable",
                table: "RfqQuotes");

            migrationBuilder.DropColumn(
                name: "SellerCompanyId",
                table: "RfqQuotes");

            migrationBuilder.AlterColumn<Guid>(
                name: "SellerCompanyId",
                table: "RfqRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
