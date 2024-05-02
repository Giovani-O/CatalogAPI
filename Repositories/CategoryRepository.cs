using CatalogAPI.Context;
using CatalogAPI.Models;
using CatalogAPI.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CatalogAPI.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {

        public CategoryRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParameters)
        {
            var categories = await GetAllAsync();
            var orderedCategories = categories.OrderBy(c => c.Id).AsQueryable();

            //var pagedCategories = PagedList<Category>.ToPagedList(
            //    orderedCategories, 
            //    categoriesParameters.PageNumber, 
            //    categoriesParameters.PageSize
            //);

            var pagedCategories = await orderedCategories.ToPagedListAsync(
                categoriesParameters.PageNumber,
                categoriesParameters.PageSize
            );

            return pagedCategories;
        }

        public async Task<IPagedList<Category>> GetCategoriesFilteredByNameAsync(CategoriesNameFilter categoriesNameFilter)
        {
            var categories = await GetAllAsync();

            if (!string.IsNullOrEmpty(categoriesNameFilter.Name))
                categories = categories.Where(c => c.Name.Contains(categoriesNameFilter.Name));

            //var filteredCategories = PagedList<Category>.ToPagedList(
            //    categories.AsQueryable(), 
            //    categoriesNameFilter.PageNumber, 
            //    categoriesNameFilter.PageSize
            //);

            var filteredCategories = await categories.ToPagedListAsync(
                categoriesNameFilter.PageNumber, 
                categoriesNameFilter.PageSize
            );

            return filteredCategories;
        }
    }
}
