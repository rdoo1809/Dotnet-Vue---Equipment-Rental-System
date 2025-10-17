using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midterm_PROG3340_RDooley.Repositories;

namespace Midterm_PROG3340_RDooley
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        // GET: /api/equipment-available
        [Authorize(Roles = "Admin,User")]
        [HttpGet("available")]
        public ActionResult<IEnumerable<Equipment>> ReadAvailableEquipment()
        {
            var available = _unitOfWork.Equipment.GetAll().Where(e => e.IsAvailable);
            var notAvailable = _unitOfWork.Equipment.GetAll().Where(e => !e.IsAvailable);
            return Ok(new { available, notAvailable });
        }
        
        // GET: /api/equipment-rented
        [Authorize(Roles = "Admin")]
        [HttpGet("rented")]
        public ActionResult<IEnumerable<Equipment>> ReadRentedEquipment()
        {
            return Ok(_unitOfWork.Equipment.GetAll().Where(e => !e.IsAvailable));
        }
        
        // GET: /api/equipment
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult<IEnumerable<Equipment>> ReadAllEquipment()
        {
            return Ok(_unitOfWork.Equipment.GetAll());
        }

        // GET: /api/equipment/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public ActionResult<Equipment> ReadOneEquipment(int id)
        {
            var equipment = _unitOfWork.Equipment.GetById(id);
            if (equipment is null) return NotFound();
            return Ok(equipment);
        }

        // POST: /api/equipment
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Equipment> CreateEquipment(Equipment equipment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _unitOfWork.Equipment.Add(equipment);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(ReadRentedEquipment), new { id = equipment.Id }, equipment);
        }

        // PUT: /api/equipment/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Equipment> UpdateEquipment(int id, Equipment incoming)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
        
            var equipment = _unitOfWork.Equipment.GetById(id);
            if (equipment is null) return NotFound();
        
            // map all fields - changing createdAt not needed
            equipment.Name = incoming.Name;
            equipment.Description = incoming.Description;
            equipment.Category = incoming.Category;
            equipment.RentalPrice = incoming.RentalPrice;
            equipment.IsAvailable = incoming.IsAvailable;
        
            _unitOfWork.Equipment.Update(equipment);
            _unitOfWork.Complete();
            return Ok(equipment);
        }
        
        // Delete: api/equipment/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<Equipment> DeleteBook(int id)
        {
            var equipment = _unitOfWork.Equipment.GetById(id);
            if (equipment is null) return NotFound();
            _unitOfWork.Equipment.Delete(equipment);
            _unitOfWork.Complete();
            return equipment;
        }
    }
}