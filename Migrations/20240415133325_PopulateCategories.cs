using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulateCategories : Migration
    {
        /// <inheritdoc />
        /// Up() is what will be executed when we run the migration
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO categories(Name, ImageUrl) VALUES('Bebidas', 'bebidas.jpg')");
            mb.Sql("INSERT INTO categories(Name, ImageUrl) VALUES('Lanches', 'lanches.jpg')");
            mb.Sql("INSERT INTO categories(Name, ImageUrl) VALUES('Sobremesas', 'sobremesas.jpg')");
        }

        /// <inheritdoc />
        /// Down() is what will run when we remove the migration
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from categories");
        }
    }
}
