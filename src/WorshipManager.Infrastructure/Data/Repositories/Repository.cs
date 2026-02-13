using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorshipManager.Core.Interfaces;
using WorshipManager.Core.Specifications;
using WorshipManager.Infrastructure.Data;
using WorshipManager.Infrastructure.Specifications;

namespace WorshipManager.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec) => await ApplySpecification(spec).ToListAsync();
    public virtual async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec) => await ApplySpecification(spec).FirstOrDefaultAsync();
    public virtual async Task<int> CountAsync(ISpecification<T> spec) => await ApplySpecification(spec).CountAsync();
    public virtual async Task<bool> AnyAsync(ISpecification<T> spec) => await ApplySpecification(spec).AnyAsync();
    protected IQueryable<T> ApplySpecification(ISpecification<T> spec) => SpecificationEvaluator.GetQuery(_dbSet.AsQueryable(), spec);

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
    public virtual async Task<T> AddAsync(T entity) { await _dbSet.AddAsync(entity); return entity; }
    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities) { await _dbSet.AddRangeAsync(entities); return entities; }
    public virtual void Update(T entity) => _dbSet.Update(entity);
    public virtual Task UpdateAsync(T entity) { _dbSet.Update(entity); return Task.CompletedTask; }
    public virtual void Delete(T entity) => _dbSet.Remove(entity);
    public virtual Task DeleteAsync(T entity) { _dbSet.Remove(entity); return Task.CompletedTask; }
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AnyAsync(predicate);
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) =>
        predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
}
