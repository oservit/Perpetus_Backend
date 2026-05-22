namespace Core.Domain.Interfaces.Common
{
    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        string ConnectionName { get; }

        Task<TEntity?> GetByIdAsync(object id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<long> InsertAsync(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        Task<int> DeleteAsync(object id);
    }
}