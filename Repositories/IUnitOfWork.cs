namespace Midterm_PROG3340_RDooley.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Equipment> Equipment { get; }
        IRepository<User> User { get; }

        int Complete();
    }
}