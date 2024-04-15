using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogAPI.Migrations
{
    /// <inheritdoc />
    public partial class CorrectProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.RenameColumn(
                name: "Stored", table: "products", newName: "Stock"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.RenameColumn(
                name: "Stock", table: "products", newName: "Stored"
            );
        }
    }
}
