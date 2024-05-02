using CatalogAPI.Models;
using CatalogAPI.Pagination;

namespace CatalogAPI.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        PagedList<Category> GetCategories(CategoriesParameters categoriesParameters);

        PagedList<Category> GetCategoriesFilteredByName(CategoriesNameFilter categoriesNameFilter);
    }
}
