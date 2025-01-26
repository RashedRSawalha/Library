using Kernal;
using LibraryManagementApplication.Authentication;
using LibraryManagementApplication.Authorization;
using LibraryManagementApplication.Queries;
using LibraryManagementApplication.RequestHandler;
using LibraryManagementApplication.Requests;
using LibraryManagementDomain.DTO;
using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Models;
using LibraryManagementInfrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Interface;
using Persistence.UnitOfWork;
using Shared.Redis;
//using LibraryManagementAPI.Data;



namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Author> _authorRepository;
        private readonly IMediator _mediator;
        private readonly IRedisCache _redisCache;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        private readonly LibraryDBContext _dBContext;
        private readonly ILogger<AuthorsController> _logger;

        //private readonly AuthorValidator _authorValidator;

        public AuthorsController(
            IUnitOfWork unitOfWork,
            IRepository<Author> authorRepository,
            IMediator mediator,
            IRedisCache redisCache,
            HttpClient httpClient,
            TokenService tokenService,
            LibraryDBContext dBContext,
            ILogger<AuthorsController> logger
            //AuthorValidator authorValidator
            )
        {
            _unitOfWork = unitOfWork;
            _authorRepository = authorRepository;
            _mediator = mediator;
            _redisCache = redisCache;
            _httpClient = httpClient;
            _tokenService = tokenService;
            _dBContext = dBContext;
            _logger = logger;
            //_authorValidator = authorValidator;
        }


        // GET: api/authors
        [HttpGet]
        [AuthorizeRoleOrPermission(
              roles: new[] { "Admin", "Viewer" },
              permissions: new[] { "GetAuthors" }
          )]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            try
            {
                var authors = await _mediator.Send(new GetAuthorsRequest());
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("filtered")]
        [ProducesResponseType(typeof(IEnumerable<AuthorDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetFilteredAuthors([FromBody] FilterRequest filterRequest)
        {
            if (filterRequest == null || string.IsNullOrWhiteSpace(filterRequest.Search))
            {
                return BadRequest("Search term is required.");
            }

            var authors = await _mediator.Send(new GetFilteredAuthorsQuery { Search = filterRequest.Search });
            return Ok(authors);
        }

        // GET: api/authors/{name}
        [HttpGet("by-name/{name}")]
        [AuthorizeRoleOrPermission(
              roles: new[] { "Admin", "Viewer" },
              permissions: new[] { "GetAuthors" }
          )]
        public async Task<ActionResult<AuthorDTO>> GetAuthorByName(string name)
        {

            //string cacheKey = $"Author:{name}";
            //string apiUrl = $"https://localhost:44328/api/authors/GetFromDB?Name={name}";

            //// Check if the author exists in the Redis cache
            //var cachedAuthor = await _redisCache.GetAsync<AuthorDTO>(cacheKey, apiUrl);
            //return Ok(cachedAuthor);

            var result = await _mediator.Send(new GetAuthorByNameRequest { AuthorName = name });

            if (result == null)
                return NotFound(new { Message = "Author not found" });

            return Ok(result);
        }

        //[Authorize]
        //Post: api/authors/post
        [HttpPost]
        [AuthorizeRoleOrPermission(
            roles: new[] { "Admin" },
            permissions: new[] { "AddAuthors" }
        )]
        public async Task<ActionResult> AddAuthor([FromBody] AuthorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var createdAuthor = await _mediator.Send(new AddAuthorRequest { Model = model });
            return CreatedAtAction(nameof(AddAuthor), new { id = createdAuthor.AuthorId }, createdAuthor);
        }


        // PUT: api/authors/{id}
        [HttpPut("{id}")]
        [AuthorizeRoleOrPermission(
            roles: new[] { "Admin" },
            permissions: new[] { "EditAuthors" }
        )]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorModel model)
        {
            var updatedAuthor = await _mediator.Send(new UpdateAuthorRequest { AuthorId = id, Model = model });
            return Ok(updatedAuthor);
        }
        
        
        [AllowAnonymous]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetPaginatedAuthors([FromBody] PaginationRequest paginationRequest)
        {
            Console.WriteLine("Controller method invoked"); // Debug log

            if (paginationRequest.PageIndex < 1 || paginationRequest.PageSize < 1)
            {
                return BadRequest("PageIndex and PageSize must be greater than 0.");
            }

            var result = await _mediator.Send(new GetPaginatedAuthorsQuery
            {
                PageIndex = paginationRequest.PageIndex,
                PageSize = paginationRequest.PageSize,
                Search = paginationRequest.Search
            });

            return Ok(result);
        }


        // DELETE: api/authors/{id}
        [HttpDelete("{id}")]
        [AuthorizeRoleOrPermission(
            roles: new[] { "Admin" },
            permissions: new[] { "DeleteAuthors" }
        )]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var deletedAuthorId = await _mediator.Send(new DeleteAuthorRequest { AuthorId = id });
            return Ok($"Author with ID {deletedAuthorId} deleted successfully.");
        }

        [AllowAnonymous]
        [HttpGet("test-exception")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult TestException()
        {
            throw new Exception("This is a test exception.");
        }


        [AllowAnonymous]
        [HttpGet("GetFromDB")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> GetFromDB([FromQuery] CacheFilter options)
        {
            if (string.IsNullOrEmpty(options.Name))
            {
                return BadRequest("Name filter is required.");
            }


            var result = await _mediator.Send(new GetAuthorByNameQuery { AuthorName = options.Name });
            if (result is not null)
                return Ok(result);

            return Ok(null);
        }

        [Authorize]
        [HttpGet("reports")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetReports()
        {
            return Ok("You have access to view reports.");
        }

        [Authorize]
        [HttpPost("edit")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult EditData()
        {
            return Ok("You have access to edit data.");
        }

    }
}


