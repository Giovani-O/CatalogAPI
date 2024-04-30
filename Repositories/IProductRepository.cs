using CatalogAPI.Models;
using CatalogAPI.Pagination;

namespace CatalogAPI.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        // IEnumerable<Product> GetProducts(ProductsParameters productsParameters);
        PagedList<Product> GetProducts(ProductsParameters productsParameters);
        IEnumerable<Product> GetProductsByCategory(int id);
    }
}
