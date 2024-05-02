namespace CatalogAPI.Repositories
{
    public interface IUnitOfWork
    {
        // Instances to access the operations of the repositories
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }

        // Method to confirm pending operations
        Task CommitAsync();
    }
}
