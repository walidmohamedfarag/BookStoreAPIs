using System.Linq.Expressions;

namespace BookStoreAPIs.Reposatories
{
    public interface IReposatory<TEntity> where TEntity : class
    {
        public Task<IEnumerable<TEntity>> GetAllAsync
            (
            Expression<Func<TEntity, bool>>? expression = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            bool tracking = true,
            CancellationToken cancellationToken = default
            );
        public Task<TEntity> GetOneAsync
            (
            Expression<Func<TEntity , bool>>? expression = null,
            Expression<Func<TEntity , object>>[]? includes = null,
            bool tracking = true,
            CancellationToken cancellationToken = default
            );
        public Task AddAsync(TEntity entity , CancellationToken cancellationToken = default);
        public void Update(TEntity entity , CancellationToken cancellationToken = default);
        public void Delete(TEntity entity , CancellationToken cancellationToken = default);
        public Task CommitAsync(CancellationToken cancellationToken = default);

    }
}
