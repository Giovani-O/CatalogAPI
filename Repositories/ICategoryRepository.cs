using CatalogAPI.Models;
using CatalogAPI.Pagination;
using X.PagedList;

namespace CatalogAPI.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParameters);

        Task<IPagedList<Category>> GetCategoriesFilteredByNameAsync(CategoriesNameFilter categoriesNameFilter);
    }
}
