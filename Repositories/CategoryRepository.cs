using CatalogAPI.Context;
using CatalogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetCategories()
        {
            if (_context.Categories is null)
                throw new InvalidOperationException("Ocorreu um problema ao buscar as categorias!");

            return _context.Categories?.ToList() ?? new List<Category>();
        }

        public Category GetCategory(int id)
        {
            if (_context.Categories is null)
                throw new InvalidOperationException("Ocorreu um problema ao buscar a categoria!");

            return _context.Categories?.FirstOrDefault(c => c.Id == id) ?? new Category();
        }

        public Category Create(Category category)
        {
            if (category is null || _context.Categories is null)
                throw new InvalidOperationException("Ocorreu um problema ao criar a categoria!");

            _context.Categories.Add(category);
            _context.SaveChanges();

            return category;
        }

        public Category Update(Category category)
        {
            if (category is null || _context.Categories is null)
                throw new InvalidOperationException("Ocorreu um problema ao atualizar a categoria!");

            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();

            return category;
        }

        public Category Delete(int id)
        {
            if (id <= 0 || _context.Categories is null)
                throw new InvalidOperationException("Ocorreu um problema ao excluir a categoria!");

            var category = _context.Categories.Find(id);

            if (category is null)
                throw new ArgumentNullException(nameof(category));

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return category;
        }
    }
}
