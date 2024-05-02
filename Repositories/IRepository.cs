using System.Linq.Expressions;

namespace CatalogAPI.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate); // Esse parâmetro representa uma função lambda
        T Create(T entity);
        T Update(T entity);
        T Delete(T entity);
    }
}
