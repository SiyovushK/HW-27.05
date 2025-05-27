using System.Linq.Expressions;

namespace Infrastructure.Interfaces;

public interface IAuthRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    Task SaveChangesAsync();
}