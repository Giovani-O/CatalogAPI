using CatalogAPI.Models;
using CatalogAPI.Pagination;
using X.PagedList;

namespace CatalogAPI.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        // IEnumerable<Product> GetProducts(ProductsParameters productsParameters);
        Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParameters);
        Task<IPagedList<Product>> GetProductsFilteredByPriceAsync(ProductsPriceFilter productsPriceFilter);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);

    }
}
