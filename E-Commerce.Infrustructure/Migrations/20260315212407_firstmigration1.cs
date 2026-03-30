using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class firstmigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryParentId",
                table: "categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_CategoryParentId",
                table: "categories",
                column: "CategoryParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_CategoryParentId",
                table: "categories",
                column: "CategoryParentId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_CategoryParentId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_CategoryParentId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "CategoryParentId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "categories");
        }
    }
}
