using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addfulltextindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create a full-text index on the 'name' column of the 'products' table
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;", suppressTransaction: true);

            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Porduct.Products(name) KEY INDEX PK_Products;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the full-text index on the 'name' column of the 'products' table
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Porduct.Products;", suppressTransaction: true);

            // Remove the full-text catalog
            migrationBuilder.Sql("DROP FULLTEXT CATALOG ftCatalog;", suppressTransaction: true);
        }
    }
}
