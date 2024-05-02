using CatalogAPI.Models;
using CatalogAPI.Pagination;

namespace CatalogAPI.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        // IEnumerable<Product> GetProducts(ProductsParameters productsParameters);
        PagedList<Product> GetProducts(ProductsParameters productsParameters);
        PagedList<Product> GetProductsFilteredByPrice(ProductsPriceFilter productsPriceFilter);
        IEnumerable<Product> GetProductsByCategory(int id);

    }
}
