using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changingcartschema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Carts",
                schema: "Cart",
                newName: "Carts",
                newSchema: "Customer");

            migrationBuilder.RenameTable(
                name: "CartItems",
                schema: "Cart",
                newName: "CartItems",
                newSchema: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cart");

            migrationBuilder.RenameTable(
                name: "Carts",
                schema: "Customer",
                newName: "Carts",
                newSchema: "Cart");

            migrationBuilder.RenameTable(
                name: "CartItems",
                schema: "Customer",
                newName: "CartItems",
                newSchema: "Cart");
        }
    }
}
