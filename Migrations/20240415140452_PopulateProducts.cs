using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulateProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO products(Name, Description, Price, ImageUrl, Stock, RegisterDate, CategoryId) " +
            "VALUES('Coca-Cola', 'Latinha 350ml', 5.45, 'cocacola.jpg', 50, now(), 1)");

            mb.Sql("INSERT INTO products(Name, Description, Price, ImageUrl, Stock, RegisterDate, CategoryId) " +
            "VALUES('Australian Burger', 'Pão australiano, hamburguer artesanal 90g, rúcula, tomate em rodelas, onion rings, bacon e queijo cheddar', 37.50, 'hamburguer.jpg', 999, now(), 2)");

            mb.Sql("INSERT INTO products(Name, Description, Price, ImageUrl, Stock, RegisterDate, CategoryId) " +
            "VALUES('Açaí', 'Copo 400ml com complementos personalizados', 14.99, 'acai.jpg', 999, now(), 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM products");
        }
    }
}
