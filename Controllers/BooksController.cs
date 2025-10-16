using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midterm_PROG3340_RDooley.Models.DTOs;
using Midterm_PROG3340_RDooley.Repositories;

namespace Midterm_PROG3340_RDooley
{
    [Authorize]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")] // api/v1/books or api/v2/books
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BooksController(IUnitOfWork unitOfWork)
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
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                IsAvailable = book.IsAvavilable
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
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                IsAvailable = book.IsAvavilable
            };
        }

        // POST: api/v1/books
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Book> CreateBook(BookV1Dto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Price = dto.Price,
                IsAvavilable = dto.IsAvailable
            };

            _unitOfWork.Books.Add(book);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // Put: api/books/v1/{id}
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Book> UpdateBook(int id, BookV1Dto bookDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();

            // only update allowed fields
            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Price = bookDto.Price;
            book.IsAvavilable = bookDto.IsAvailable;

            _unitOfWork.Books.Update(book);
            _unitOfWork.Complete();
            return Ok(book);
        }

        // *** v2 routes *** //
        // GET: /api/v2/books
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooksV2()
        {
            return Ok(_unitOfWork.Books.GetAll());
        }

        // GET: /api/v2/books/{id}
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public ActionResult<Book> GetBookV2(int id)
        {
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();
            return Ok(book);
        }

        // POST: /api/v2/books
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Book> CreateBookV2(Book book)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _unitOfWork.Books.Add(book);
            _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: /api/v2/books/{id}
        [MapToApiVersion("2.0")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<Book> UpdateBookV2(int id, Book incoming)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
        
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();
        
            // map all fields (v2 allows Genre/PublishedYear)
            book.Title = incoming.Title;
            book.Author = incoming.Author;
            book.Price = incoming.Price;
            book.IsAvavilable = incoming.IsAvavilable;
            book.Genre = incoming.Genre;
            book.PublishedYear = incoming.PublishedYear;
        
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
        public ActionResult<Book> DeleteBook(int id)
        {
            var book = _unitOfWork.Books.GetById(id);
            if (book is null) return NotFound();
            _unitOfWork.Books.Delete(book);
            _unitOfWork.Complete();
            return book;
        }
    }
}