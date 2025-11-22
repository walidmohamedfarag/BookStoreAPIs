using BookStoreAPIs.DataAccess;
using System.Linq.Expressions;

namespace BookStoreAPIs.Reposatories
{
    public class Reposatory<TEntity> : IReposatory<TEntity> where TEntity : class
    {
        private readonly ApplicationDBContext context;
        private DbSet<TEntity> dbSet;
        private readonly ILogger<Reposatory<TEntity>> logger;
        public Reposatory(ApplicationDBContext _context, ILogger<Reposatory<TEntity>> _logger)
        {
            context = _context;
            dbSet = context.Set<TEntity>();
            logger = _logger;
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
            //try
            //{
            //}
            //catch(Exception ex)
            //{
            //    logger.LogError($"Error: {ex.Message}");
            //}
        }

        public void Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            context.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null, Expression<Func<TEntity, object>>[]? includes = null, bool tracking = true, CancellationToken cancellationToken = default)
        {
            var query = dbSet.AsQueryable();
            if (expression is not null)
                query = query.Where(expression);
            if (!tracking)
                query = query.AsNoTracking();
            if (includes is not null)
                foreach(var include in includes)
                    query = query.Include(include);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>>? expression = null, Expression<Func<TEntity, object>>[]? includes = null, bool tracking = true, CancellationToken cancellationToken = default)
        {
            return (await GetAllAsync(expression, includes, tracking, cancellationToken)).FirstOrDefault();
        }

        public void Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            dbSet.Update(entity);
        }
    }
}
