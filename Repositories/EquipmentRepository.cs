using Microsoft.EntityFrameworkCore;
using Midterm_PROG3340_RDooley.Data;

namespace Midterm_PROG3340_RDooley.Repositories;

public class EquipmentRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public EquipmentRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }
    
    public IEnumerable<TEntity> GetAll() => _dbSet.ToList();

    public TEntity? GetById(int id) => _dbSet.Find(id);

    public void Add(TEntity model) => _dbSet.Add(model);

    public void Update(TEntity model) => _dbSet.Update(model);

    public void Delete(TEntity model) => _dbSet.Remove(model);
}