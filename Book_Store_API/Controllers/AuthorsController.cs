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
    /// Endpoint used to interact with Authors in the books store's db
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository authorRepo_;
        ILoggerService looger_;
        IMapper mapper_;
        public AuthorsController(IAuthorRepository authorRepo,
            ILoggerService logger, IMapper mapper)
        {
            authorRepo_ = authorRepo;
            looger_ = logger;
            mapper_ = mapper;
        }
        /// <summary>
        /// GetAllAuthors
        /// </summary>
        /// <returns>List Of Authors</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                looger_.LogInfo("Calling GetAuthors");
                var authors = await authorRepo_.FindAll();
                var response = mapper_.Map<IList<AuthorDTO>>(authors);
                looger_.LogInfo("Returned fine GetAuthors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// GetAuthor
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Author beloning to id</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                looger_.LogInfo($"Calling GetAuthor for specific id:{id}");
                var author = await authorRepo_.FindById(id);
                if (author == null)
                {
                    looger_.LogWarn($"Author with id:{id} not found.");
                    return NotFound();
                }
                var response = mapper_.Map<AuthorDTO>(author);
                looger_.LogInfo("Author Returned successfully.");
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
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles ="Administrator,Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                looger_.LogInfo($"Calling GetAuthor for specific id:{id}");
                if (id < 0 || authorDTO == null || id != authorDTO.Id)
                    return BadRequest();
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var author = mapper_.Map<Author>(authorDTO);
                var isExists = await authorRepo_.IsExists(id);
                if(!isExists)
                    return NotFound();
                var success = await authorRepo_.Update(author);
                looger_.LogInfo("Author Updated successfully.");
                if(!success)
                    return InternalError($"Update Failed.");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Creates Author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDTO authorDTO)
        {
            try
            {
                if (authorDTO == null)
                {
                    looger_.LogInfo("Create Author Requested.");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    looger_.LogInfo("Author data provided is not correct.");
                    return BadRequest(ModelState);
                }
                var author = mapper_.Map<Author>(authorDTO);
                var success = await authorRepo_.Create(author);
                if (!success)
                {
                    return InternalError("Author Creation Failed.");
                }
                return Created("Create", new { author });
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles ="Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                if (id < 0)
                    return BadRequest();
                looger_.LogInfo($"Calling GetAuthor for specific id:{id}");
                var author = await authorRepo_.FindById(id);
                if (author == null)
                    return NotFound();
                bool success = await authorRepo_.Delete(author);
                if(!success)
                    return InternalError($"Couldn't delete the record.");
                looger_.LogInfo("Author deleted successfully.");
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
