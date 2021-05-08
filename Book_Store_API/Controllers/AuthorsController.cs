using AutoMapper;
using Book_Store_API.Contract;
using Book_Store_API.DTOs;
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
            catch(Exception e)
            {
                looger_.LogError($"{e.Message} - {e.InnerException}");
                return StatusCode(500, "Something went wrong. Please contact administrator.");
            }
        }
    }
}
