using CatalogAPI.Models;

namespace CatalogAPI.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetProducts();
        Product GetProduct(int id);
        Product Create(Product product);
        bool Update(Product product);
        bool Delete(int id);
    }
}
