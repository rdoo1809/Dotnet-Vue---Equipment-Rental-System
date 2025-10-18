using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midterm_PROG3340_RDooley.Repositories;
using Midterm_PROG3340_RDooley.Services;

namespace Midterm_PROG3340_RDooley
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CustomerService _customerService;

        public RentalController(IUnitOfWork unitOfWork, CustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
        }
        
        // GET: /api/rental
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult<IEnumerable<Rental>> ReadAllRentals()
        {
            var (userName, userRole) = _customerService.GetUserNameAndRole(User);
            var customer = _unitOfWork.Customer.GetAll()
                .FirstOrDefault(c => c.UserName == userName);
            if (customer is null) return NotFound();
            var rentals = _unitOfWork.Rental.GetAll();
            
            //user role can only view own data
            if (userRole == "User") return Ok(rentals.Where(r => r.CustomerId == customer.Id).ToList());
            return Ok(rentals);
        }

        // GET: /api/rental/active
        [Authorize(Roles = "Admin,User")]
        [HttpGet("active")]
        public ActionResult<IEnumerable<Rental>> ReadActiveRentals()
        {
            var (userName, userRole) = _customerService.GetUserNameAndRole(User);
            var customer = _unitOfWork.Customer.GetAll()
                .FirstOrDefault(c => c.UserName == userName);
            if (customer is null) return NotFound();
            
            var activeRentals = _unitOfWork.Rental.GetAll()
                .Where(r => r.ReturnedAt == null);
            
            //user role can only view own data
            if (userRole == "User") 
                return Ok(activeRentals.Where(r => r.CustomerId == customer.Id).ToList());
            
            return Ok(activeRentals);
        }

        // GET: /api/rental/completed
        [Authorize(Roles = "Admin,User")]
        [HttpGet("completed")]
        public ActionResult<IEnumerable<Rental>> ReadCompletedRentals()
        {
            var (userName, userRole) = _customerService.GetUserNameAndRole(User);
            var customer = _unitOfWork.Customer.GetAll()
                .FirstOrDefault(c => c.UserName == userName);
            if (customer is null) return NotFound();
            
            var completedRentals = _unitOfWork.Rental.GetAll()
                .Where(r => r.ReturnedAt != null);
            
            //user role can only view own data
            if (userRole == "User") 
                return Ok(completedRentals.Where(r => r.CustomerId == customer.Id).ToList());
            
            return Ok(completedRentals);
        }

        // GET: /api/rental/overdue
        [Authorize(Roles = "Admin,User")]
        [HttpGet("overdue")]
        public ActionResult<IEnumerable<Rental>> ReadOverdueRentals()
        {
            var (userName, userRole) = _customerService.GetUserNameAndRole(User);
            var customer = _unitOfWork.Customer.GetAll()
                .FirstOrDefault(c => c.UserName == userName);
            if (customer is null) return NotFound();
            
            var overdueRentals = _unitOfWork.Rental.GetAll()
                .Where(r => r.DueDate < DateTime.Now && r.ReturnedAt == null);
            
            //user role can only view own data
            if (userRole == "User") 
                return Ok(overdueRentals.Where(r => r.CustomerId == customer.Id).ToList());
            
            return Ok(overdueRentals);
        }

        // GET: /api/rental/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public ActionResult<Rental> ReadOneRental(int id)
        {
            var customer = _unitOfWork.Customer.GetById(id);
            var rental = _unitOfWork.Rental.GetById(id);
            if (customer is null || rental is null) return NotFound();
            
            if (customer.Role == "User") return rental.CustomerId != customer.Id ? Forbid() : Ok(rental);
            return Ok(rental);
        }

        // POST: /api/rental/issue
        [Authorize(Roles = "Admin,User")]
        [HttpPost("issue")]
        public ActionResult<Equipment> CreateRentalIssue(Rental rental)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var (userName, userRole) = _customerService.GetUserNameAndRole(User);
            var loggedInCustomer = _unitOfWork.Customer.GetAll()
                    .FirstOrDefault(c => c.UserName == userName);
            if (loggedInCustomer is null) return NotFound("Authenticated User not found");
            
            var customer = _unitOfWork.Customer.GetById(rental.CustomerId);
            if (customer is null) return NotFound("Customer not found");
            
            //user can only issue for themselves
            if (loggedInCustomer.Role == "User" && loggedInCustomer.Id != rental.CustomerId)
                return Forbid(); 
            
            //universal rules
                //customer can only have one acive rental
                //equipment has to be available
            var hasActiveRental = HasActiveRental(customer); 
            var equipmentAvailable = CheckEquipmentAvailability(rental);
            if (hasActiveRental) return BadRequest("Customer has an active rental.");
            if (!equipmentAvailable) return BadRequest("Equipment is not available.");
            
            //mark equipment as unavailable
            SetEquipmentAvailability(rental, false);
            
            //add rental
            rental.IssuedAt = DateTime.UtcNow;
            rental.DueDate = DateTime.UtcNow.AddDays(7); // Set due date to 7 days from issue
            rental.ReturnedAt = null;
            _unitOfWork.Rental.Add(rental);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(ReadOneRental), new { id = rental.CustomerId }, rental);
        }

        // POST: /api/rental/return
        [Authorize(Roles = "Admin,User")]
        [HttpPost("return")]
        public ActionResult<Equipment> CreateRentalReturn(Rental rental)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var (userName, userRole) = _customerService.GetUserNameAndRole(User);
            var loggedInCustomer = _unitOfWork.Customer.GetAll()
                .FirstOrDefault(c => c.UserName == userName);
            if (loggedInCustomer is null) return NotFound("Authenticated User not found");
            
            var customer = _unitOfWork.Customer.GetById(rental.CustomerId);
            if (customer is null) return NotFound("Customer not found");
            
            //user can only return for themselves
            if (loggedInCustomer.Role == "User" && loggedInCustomer.Id != rental.CustomerId)
                return Forbid(); 
            
            //mark equipment as available
            SetEquipmentAvailability(rental, true);
            
            //update rental as returned
            rental.ReturnedAt = DateTime.UtcNow;
            _unitOfWork.Rental.Update(rental);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(ReadOneRental), new { id = rental.CustomerId }, rental);
        }
        
        // PUT: /api/rental/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Equipment> UpdateRental(int id, Rental incoming)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var rental = _unitOfWork.Rental.GetById(id);
            if (rental is null) return NotFound();
        
            rental.DueDate = incoming.DueDate;
            _unitOfWork.Rental.Update(rental);
            _unitOfWork.Complete();
            return Ok(incoming.DueDate);
        }
        
        // Delete: api/rental/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<Rental> DeleteRental(int id)
        {
            var rental = _unitOfWork.Rental.GetById(id);
            if (rental is null) return NotFound();
            _unitOfWork.Rental.Delete(rental);
            _unitOfWork.Complete();
            return rental;
        }
        
        //helper methods
        private void SetEquipmentAvailability(Rental rental, bool isAvailable)
        {
            var equipment  = _unitOfWork.Equipment.GetById(rental.EquipmentId);
            if (equipment == null) return;
            equipment.IsAvailable = isAvailable;
            _unitOfWork.Equipment.Update(equipment);
        }

        private bool HasActiveRental(Customer customer)
        {
            return _unitOfWork.Rental.GetAll()
                .Any(r => r.CustomerId == customer.Id && r.ReturnedAt == null);
        }
        
        private bool CheckEquipmentAvailability(Rental rental)
        {
            var equipment = _unitOfWork.Equipment.GetById(rental.EquipmentId);
            if (equipment == null) return false;
            return equipment.IsAvailable;
        }
    }
}