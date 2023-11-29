using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class modifycustomermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Customer",
                table: "Customers");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                schema: "Customer",
                table: "Customers",
                column: "Email",
                unique: true);
        }
    }
}
