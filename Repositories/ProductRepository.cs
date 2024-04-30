﻿using CatalogAPI.Context;
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

        public IEnumerable<Product> GetProductsByCategory(int id)
        {
            return GetAll().Where(c => c.CategoryId == id);
        }
    }
}
