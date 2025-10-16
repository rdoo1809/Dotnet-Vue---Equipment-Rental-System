using Midterm_PROG3340_RDooley.Repositories;
using Midterm_PROG3340_RDooley.Data;

namespace Midterm_PROG3340_RDooley.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public IRepository<Book> Books { get; set; }

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            Books = new BookRepository<Book>(_appDbContext);
        }
        
        public int Complete()
        {
            return _appDbContext.SaveChanges();
        }
    }
}
