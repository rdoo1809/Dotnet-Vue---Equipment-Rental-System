using Midterm_PROG3340_RDooley.Repositories;
using Midterm_PROG3340_RDooley.Data;

namespace Midterm_PROG3340_RDooley.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public IRepository<Equipment> Equipment { get; set; }
        public IRepository<User> User { get; set; }

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            Equipment = new EquipmentRepository<Equipment>(_appDbContext);
            User = new UserRepository<User>(_appDbContext);
        }
        
        public int Complete()
        {
            return _appDbContext.SaveChanges();
        }
    }
}
