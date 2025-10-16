using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midterm_PROG3340_RDooley.Models.DTOs;
using Midterm_PROG3340_RDooley.Repositories;

namespace Midterm_PROG3340_RDooley
{
    [Authorize]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")] // api/v1/equipment or api/v2/equipment
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EquipmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // *** v1 routes *** //
        // GET /api/v1/books
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult<IEnumerable<BookV1Dto>> GetBooksV1()
        {
            var books = _unitOfWork.Books.GetAll().Select(book => new BookV1Dto
            {
                Title = book.Name,
                Author = book.Description,
                // Price = book.Category,
                // IsAvailable = book.RentalPrice
            });
            
            return Ok(books);
        }

        // GET: /api/v1/books/{id}
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public ActionResult<BookV1Dto>? GetBook(int id)
        {
            var book = _unitOfWork.Books.GetById(id);
            if (book == null) return NotFound();
            
            return new BookV1Dto
            {
                Title = book.Name,
                Author = book.Description,
                // Price = book.Category,
                // IsAvailable = book.RentalPrice
            };
        }

        // POST: api/v1/books
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Equipment> CreateBook(BookV1Dto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var book = new Equipment
            {
                Name = dto.Title,
                Description = dto.Author,
                // Category = dto.Price,
                // RentalPrice = dto.IsAvailable
            };

            _unitOfWork.Books.Add(book);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // Put: api/books/v1/{id}
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Equipment> UpdateBook(int id, BookV1Dto bookDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();

            // only update allowed fields
            book.Name = bookDto.Title;
            book.Description = bookDto.Author;
            // book.Category = bookDto.Price;
            // book.RentalPrice = bookDto.IsAvailable;

            _unitOfWork.Books.Update(book);
            _unitOfWork.Complete();
            return Ok(book);
        }

        // *** v2 routes *** //
        // GET: /api/v2/books
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult<IEnumerable<Equipment>> GetBooksV2()
        {
            return Ok(_unitOfWork.Books.GetAll());
        }

        // GET: /api/v2/books/{id}
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public ActionResult<Equipment> GetBookV2(int id)
        {
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();
            return Ok(book);
        }

        // POST: /api/v2/books
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Equipment> CreateBookV2(Equipment equipment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _unitOfWork.Books.Add(equipment);
            _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetBook), new { id = equipment.Id }, equipment);
        }

        // PUT: /api/v2/books/{id}
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Equipment> UpdateBookV2(int id, Equipment incoming)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
        
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();
        
            // map all fields (v2 allows Genre/PublishedYear)
            book.Name = incoming.Name;
            book.Description = incoming.Description;
            book.Category = incoming.Category;
            book.RentalPrice = incoming.RentalPrice;
            book.IsAvailable = incoming.IsAvailable;
            // book.PublishedYear = incoming.PublishedYear;
        
            _unitOfWork.Books.Update(book);
            _unitOfWork.Complete();
        
            return Ok(book);
        }
        
        // *** universal routes *** //
        // Delete: api/books/v#/{id}
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<Equipment> DeleteBook(int id)
        {
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();
            _unitOfWork.Books.Delete(book);
            _unitOfWork.Complete();
            return book;
        }
    }
}