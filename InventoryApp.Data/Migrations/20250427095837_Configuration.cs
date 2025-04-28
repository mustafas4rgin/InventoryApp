using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Suppliers_SupplierId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SupplierId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SupplierId1",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId1",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SupplierId1",
                table: "Users",
                column: "SupplierId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Suppliers_SupplierId1",
                table: "Users",
                column: "SupplierId1",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }
    }
}
