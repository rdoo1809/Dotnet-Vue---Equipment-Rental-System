namespace Midterm_PROG3340_RDooley.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Equipment> Books { get; }

        int Complete();
    }
}