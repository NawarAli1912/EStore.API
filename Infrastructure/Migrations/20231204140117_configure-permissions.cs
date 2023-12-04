using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class configurepermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Building",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_City",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_County",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "Order",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "LineItems",
                schema: "Order",
                newName: "LineItems");

            migrationBuilder.RenameTable(
                name: "Customers",
                schema: "Customer",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "Category",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "Carts",
                schema: "Customer",
                newName: "Carts");

            migrationBuilder.RenameTable(
                name: "CartItems",
                schema: "Customer",
                newName: "CartItems");

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRole",
                columns: table => new
                {
                    PermissionsId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRole", x => new { x.PermissionsId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_PermissionRole_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRole_Permission_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "526d78ee-a36e-4f42-9a03-65d2ef7a12ef", "06e4dcf7-8601-467c-a3aa-3af3070b0275", "Admin", "ADMIN" },
                    { "ff036f19-a265-4432-85c2-e290d3c48396", "2632e755-d72f-451e-a881-36dba2b8d0eb", "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { -7, "All" },
                    { -6, "ManageRoles" },
                    { -5, "ViewRoles" },
                    { -4, "ConfigureAccessControl" },
                    { -3, "CreateProduct" },
                    { -2, "ReadDetails" },
                    { -1, "None" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Id",
                table: "Permission",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRole_RoleId",
                table: "PermissionRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionRole");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "526d78ee-a36e-4f42-9a03-65d2ef7a12ef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ff036f19-a265-4432-85c2-e290d3c48396");

            migrationBuilder.EnsureSchema(
                name: "Customer");

            migrationBuilder.EnsureSchema(
                name: "Category");

            migrationBuilder.EnsureSchema(
                name: "Order");

            migrationBuilder.EnsureSchema(
                name: "Product");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "Product");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Orders",
                newSchema: "Order");

            migrationBuilder.RenameTable(
                name: "LineItems",
                newName: "LineItems",
                newSchema: "Order");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customers",
                newSchema: "Customer");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Categories",
                newSchema: "Category");

            migrationBuilder.RenameTable(
                name: "Carts",
                newName: "Carts",
                newSchema: "Customer");

            migrationBuilder.RenameTable(
                name: "CartItems",
                newName: "CartItems",
                newSchema: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "Address_Building",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_County",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
