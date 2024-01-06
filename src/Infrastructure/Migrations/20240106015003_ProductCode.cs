using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FriendlyIdSequences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ProductSequence = table.Column<int>(type: "int", nullable: false),
                    OrderSequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendlyIdSequences", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FriendlyIdSequences",
                columns: new[] { "Id", "OrderSequence", "ProductSequence" },
                values: new object[] { 1, 4202, 2024 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendlyIdSequences");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Products");
        }
    }
}
