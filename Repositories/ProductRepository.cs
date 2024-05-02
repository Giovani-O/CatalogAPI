using CatalogAPI.Context;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using X.PagedList;

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

        public async Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParameters)
        {
            var products = await GetAllAsync();
            var orderedProducts = products.OrderBy(p => p.Id).AsQueryable();
            var result = await orderedProducts.ToPagedListAsync(
                productsParameters.PageNumber,
                productsParameters.PageSize
            );

            return result;
        }

        public async Task<IPagedList<Product>> GetProductsFilteredByPriceAsync(ProductsPriceFilter productsPriceFilter)
        {
            var products = await GetAllAsync();

            if (productsPriceFilter.Price.HasValue && !string.IsNullOrEmpty(productsPriceFilter.PriceCondition))
            {
                if (productsPriceFilter.PriceCondition.Equals("maior", StringComparison.OrdinalIgnoreCase))
                    products = products.Where(p => p.Price > productsPriceFilter.Price.Value).OrderBy(p => p.Price);
                else if (productsPriceFilter.PriceCondition.Equals("menor", StringComparison.OrdinalIgnoreCase))
                    products = products.Where(p => p.Price < productsPriceFilter.Price.Value).OrderBy(p => p.Price);
                else if (productsPriceFilter.PriceCondition.Equals("igual", StringComparison.OrdinalIgnoreCase))
                    products = products.Where(p => p.Price == productsPriceFilter.Price.Value).OrderBy(p => p.Price);
            }

            var fiteredProducts = await products.ToPagedListAsync(
                productsPriceFilter.PageNumber, 
                productsPriceFilter.PageSize
            );

            return fiteredProducts;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
        {
            var products = await GetAllAsync();
            var productsCategory = products.Where(c => c.CategoryId == id);

            return productsCategory;
        }

    }
}
