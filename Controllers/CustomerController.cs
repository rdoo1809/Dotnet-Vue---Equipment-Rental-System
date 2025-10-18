using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midterm_PROG3340_RDooley.Repositories;
using Midterm_PROG3340_RDooley.Services;

namespace Midterm_PROG3340_RDooley
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CustomerService _customerService;

        public CustomerController(IUnitOfWork unitOfWork, CustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
        }
        
        //TODO
        // GET: /api/customer/{id}/rental-history
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}/rental-history")]
        public ActionResult<IEnumerable<Equipment>> ReadCustomerRentalHistory(int id)
        {
            return Ok(_unitOfWork.Customer.GetAll());
        }
        
        //TODO
        // GET: /api/customer/{id}/active-rental
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}/active-rental")]
        public ActionResult<IEnumerable<Equipment>> ReadCustomerActiveRentals(int id)
        {
            return Ok(_unitOfWork.Customer.GetAll());
        }
        
        // GET: /api/customer
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<Equipment>> ReadAllCustomers()
        {
            return Ok(_unitOfWork.Customer.GetAll());
        }

        // GET: /api/customer/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public ActionResult<Equipment> ReadOneCustomer(int id)
        {
            var (customerName, customerRole) = _customerService.GetUserNameAndRole(User);
            var customer = _unitOfWork.Customer.GetById(id);
            if (customer is null) return NotFound();
            
            //user role can only view own data
            if (customerRole == "User") return customer.UserName != customerName ? Forbid() : Ok(customer);
            return Ok(customer);
        }

        // POST: /api/customer
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Customer> CreateCustomer(Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _unitOfWork.Customer.Add(customer);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(ReadOneCustomer), new { id = customer.Id }, customer);
        }

        // PUT: /api/customer/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public ActionResult<Customer> UpdateCustomer(int id, Customer incoming)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var customer = _unitOfWork.Customer.GetById(id);
            if (customer is null) return NotFound();
            var (customerName, customerRole) = _customerService.GetUserNameAndRole(User);

            //users can only update their own name password email
            if (customerRole == "User")
            {
                if (customer.UserName != customerName) return Forbid();
                customer.UserName = incoming.UserName;
                customer.Password = incoming.Password;
                customer.Email = incoming.Email;
            }
            else  //admin can edit any field of any user
            {
                customer.UserName = incoming.UserName;
                customer.Password = incoming.Password;
                customer.Email = incoming.Email;
                customer.Role = incoming.Role;   
            }

            _unitOfWork.Customer.Update(customer);
            _unitOfWork.Complete();
            return Ok(customer);
        }

        // Delete: api/equipment/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<Customer> DeleteCustomer(int id)
        {
            var customer = _unitOfWork.Customer.GetById(id);
            if (customer is null) return NotFound();
            _unitOfWork.Customer.Delete(customer);
            _unitOfWork.Complete();
            return customer;
        }
    }
}