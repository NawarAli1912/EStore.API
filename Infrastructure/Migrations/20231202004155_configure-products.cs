using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class configureproducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Product");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "Porduct",
                newName: "Products",
                newSchema: "Product");

            migrationBuilder.RenameColumn(
                name: "Price_Value",
                schema: "Product",
                table: "Products",
                newName: "PurchasePrice_Value");

            migrationBuilder.RenameColumn(
                name: "Price_Currency",
                schema: "Product",
                table: "Products",
                newName: "PurchasePrice_Currency");

            migrationBuilder.AddColumn<int>(
                name: "CustomerPrice_Currency",
                schema: "Product",
                table: "Products",
                type: "int",
                maxLength: 3,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerPrice_Value",
                schema: "Product",
                table: "Products",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPrice_Currency",
                schema: "Product",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomerPrice_Value",
                schema: "Product",
                table: "Products");

            migrationBuilder.EnsureSchema(
                name: "Porduct");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "Product",
                newName: "Products",
                newSchema: "Porduct");

            migrationBuilder.RenameColumn(
                name: "PurchasePrice_Value",
                schema: "Porduct",
                table: "Products",
                newName: "Price_Value");

            migrationBuilder.RenameColumn(
                name: "PurchasePrice_Currency",
                schema: "Porduct",
                table: "Products",
                newName: "Price_Currency");
        }
    }
}
