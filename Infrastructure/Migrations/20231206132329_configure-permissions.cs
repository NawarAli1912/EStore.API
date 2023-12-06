using System;
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
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

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
                    { "ac40d7bd-5e9e-488d-9396-6e5fce4a47a2", "efa2fb27-fff8-43b0-8f50-b14a4e92633d", "Customer", "CUSTOMER" },
                    { "c086ce56-fab0-4d51-8f25-96b66f4c0ac8", "4055f3cb-76a5-44f1-b528-7ef74f7eefff", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { -10, "All" },
                    { -9, "ConfigureAccessControl" },
                    { -8, "ManageRoles" },
                    { -7, "ManageCustomers" },
                    { -6, "ManageOrders" },
                    { -5, "ManageCarts" },
                    { -4, "ManageCategories" },
                    { -3, "ManageProducts" },
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
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "PermissionRole");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ac40d7bd-5e9e-488d-9396-6e5fce4a47a2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c086ce56-fab0-4d51-8f25-96b66f4c0ac8");

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
