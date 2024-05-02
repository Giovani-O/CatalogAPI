using CatalogAPI.Context;
using CatalogAPI.Models;
using CatalogAPI.Pagination;

namespace CatalogAPI.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {

        }

        //public IEnumerable<Product> GetProducts(ProductsParameters productsParameters)
        //{
        //    return GetAll()
        //        .OrderBy(p => p.Name)
        //        .Skip((productsParameters.PageNumber - 1) * productsParameters.PageSize)
        //        .Take(productsParameters.PageSize).ToList();
        //}

        public PagedList<Product> GetProducts(ProductsParameters productsParameters)
        {
            var products =  GetAll().OrderBy(p => p.Id).AsQueryable();
            var orderedProducts = PagedList<Product>.ToPagedList(
                products, 
                productsParameters.PageNumber, 
                productsParameters.PageSize
            );

            return orderedProducts;
        }

        public PagedList<Product> GetProductsFilteredByPrice(ProductsPriceFilter productsPriceFilter)
        {
            var products = GetAll().AsQueryable();

            if (productsPriceFilter.Price.HasValue && !string.IsNullOrEmpty(productsPriceFilter.PriceCondition))
            {
                if (productsPriceFilter.PriceCondition.Equals("maior", StringComparison.OrdinalIgnoreCase))
                    products = products.Where(p => p.Price > productsPriceFilter.Price.Value).OrderBy(p => p.Price);
                else if (productsPriceFilter.PriceCondition.Equals("menor", StringComparison.OrdinalIgnoreCase))
                    products = products.Where(p => p.Price < productsPriceFilter.Price.Value).OrderBy(p => p.Price);
                else if (productsPriceFilter.PriceCondition.Equals("igual", StringComparison.OrdinalIgnoreCase))
                    products = products.Where(p => p.Price == productsPriceFilter.Price.Value).OrderBy(p => p.Price);
            }

            var fiteredProducts = PagedList<Product>.ToPagedList(
                products, 
                productsPriceFilter.PageNumber, 
                productsPriceFilter.PageSize
            );

            return fiteredProducts;
        }

        public IEnumerable<Product> GetProductsByCategory(int id)
        {
            return GetAll().Where(c => c.CategoryId == id);
        }

    }
}
