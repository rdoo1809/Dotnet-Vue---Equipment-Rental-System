namespace Midterm_PROG3340_RDooley.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> GetAll();
    TEntity? GetById(int id);
    void Add(TEntity model);
    void Update(TEntity model);
    void Delete(TEntity model);
}