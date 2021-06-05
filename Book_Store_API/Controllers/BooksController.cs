using AutoMapper;
using Book_Store_API.Contract;
using Book_Store_API.Data;
using Book_Store_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store_API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository bookRepo_;
        ILoggerService looger_;
        IMapper mapper_;
        public BooksController(IBookRepository bookRepo,
            ILoggerService logger, IMapper mapper)
        {
            bookRepo_ = bookRepo;
            looger_ = logger;
            mapper_ = mapper;
        }
        /// <summary>
        /// GetAllBooks
        /// </summary>
        /// <returns>List Of Books</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                looger_.LogInfo("Calling GetAllBooks");
                var books = await bookRepo_.FindAll();
                var response = mapper_.Map<IList<BookDTO>>(books);
                looger_.LogInfo("Returned fine GetAllBooks");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// GetBook
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Book beloning to id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                looger_.LogInfo($"Calling GetBook for specific id:{id}");
                var book = await bookRepo_.FindById(id);
                if (book == null)
                {
                    looger_.LogWarn($"Book with id:{id} not found.");
                    return NotFound();
                }
                var response = mapper_.Map<BookDTO>(book);
                looger_.LogInfo("Book Returned successfully.");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError(e.Message);
            }
        }
        /// <summary>
        /// Updates an existing entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDTO bookDTO)
        {
            try
            {
                looger_.LogInfo($"Calling UpdateBook for specific id:{id}");
                if (id < 0 || bookDTO == null || id != bookDTO.Id)
                    return BadRequest();
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var book = mapper_.Map<Book>(bookDTO);
                var isExists = await bookRepo_.IsExists(id);
                if (!isExists)
                    return NotFound();
                var success = await bookRepo_.Update(book);
                looger_.LogInfo("Book Updated successfully.");
                if (!success)
                    return InternalError($"Update Failed.");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Creates Books
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDTO bookDTO)
        {
            try
            {
                if (bookDTO == null)
                {
                    looger_.LogInfo("Create Book Requested.");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    looger_.LogInfo("Book data provided is not correct.");
                    return BadRequest(ModelState);
                }
                var book = mapper_.Map<Book>(bookDTO);
                var success = await bookRepo_.Create(book);
                if (!success)
                {
                    return InternalError("Book Creation Failed.");
                }
                return Created("Create", new { book });
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// DeleteBook
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                if (id < 0)
                    return BadRequest();
                looger_.LogInfo($"Calling DeleteBook for specific id:{id}");
                var book = await bookRepo_.FindById(id);
                if (book == null)
                    return NotFound();
                bool success = await bookRepo_.Delete(book);
                if (!success)
                    return InternalError($"Couldn't delete the record.");
                looger_.LogInfo("Book deleted successfully.");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        private ObjectResult InternalError(string message)
        {
            looger_.LogError(message);
            return StatusCode(500, "Something Went Wrong, Please contact admin.");
        }
    }
}
