using CatalogAPI.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CatalogAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            // Nesse contexto, Set<T>() serve para acessar uma coleção do tipo T
            return _context.Set<T>().AsNoTracking().ToList();
        }

        public T? Get(Expression<Func<T, bool>> predicate)
        {
            // predicate representa a função lambda
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public T Create(T entity)
        {
            _context.Set<T>().Add(entity);
            //_context.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            // Esta opção define o estado da entidade como modificada
            //_context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            // Esta opção vai atualizar TODAS as colunas do registro no banco 
            _context.Set<T>().Update(entity);
            //_context.SaveChanges();
            return entity;


        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            //_context.SaveChanges();
            return entity;
        }
    }
}
