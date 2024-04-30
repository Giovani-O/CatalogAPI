using CatalogAPI.Context;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        public CategoryRepository(AppDbContext context) : base(context)
        {
            
        }

        public PagedList<Category> GetCategories(CategoriesParameters categoriesParameters)
        {
            var categories = GetAll().OrderBy(c => c.Id).AsQueryable();
            var orderedCategories = PagedList<Category>.ToPagedList(
                categories, 
                categoriesParameters.PageNumber, 
                categoriesParameters.PageSize
            );

            return orderedCategories;
        }
    }
}
