namespace Midterm_PROG3340_RDooley.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Equipment> Equipment { get; }
        IRepository<Customer> Customer { get; }
        IRepository<Rental> Rental { get; }

        int Complete();
    }
}