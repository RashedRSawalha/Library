using Kernal;
using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Models;
using LibraryManagementApplication.Authentication;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.DTO;
using LibraryManagementApplication.Queries;
using Persistence.Interface;
using Persistence.UnitOfWork;
using LibraryManagementApplication.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Redis;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementInfrastructure;
using System.Web.Helpers;
using BCrypt.Net;
//using LibraryManagementAPI.Data;
using Shared.AuthorizationMiddleware;
using System.ComponentModel.DataAnnotations;


namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class RoleManagegementController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Author> _authorRepository;
        private readonly IMediator _mediator;
        private readonly IRedisCache _redisCache;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        private readonly LibraryDBContext _dBContext;

        //private readonly AuthorValidator _authorValidator;

        public RoleManagegementController(
            IUnitOfWork unitOfWork,
            IRepository<Author> authorRepository,
            IMediator mediator,
            IRedisCache redisCache,
            HttpClient httpClient,
            TokenService tokenService,
            LibraryDBContext dBContext
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
            //_authorValidator = authorValidator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Fetch user from the database
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Invalid user ID or password." });
            }

            // Fetch the user's role
            var role = await _dBContext.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
            if (role == null)
            {
                return Unauthorized(new { Message = "User has no valid role assigned." });
            }

            // Fetch permissions for the user's role
            var permissions = await _dBContext.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.Permission)
                .ToListAsync();

            // Generate JWT token including user role and permissions
            var token = _tokenService.GenerateToken(user.Id, role.Name, permissions);

            return Ok(new { Token = token });
        }



        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Check if the role exists
            var role = await _dBContext.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);
            if (role == null)
            {
                return BadRequest("Invalid RoleId.");
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create a new user
            var user = new User
            {
                PasswordHash = hashedPassword,
                RoleId = model.RoleId // Set RoleId
            };

            _dBContext.Users.Add(user);
            await _dBContext.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully.", UserId = user.Id });
        }

        [AllowAnonymous]
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            try
            {
                var claims = _tokenService.ParseToken(token);

                if (claims == null)
                    return Unauthorized(new { Message = "Invalid or expired token." });

                // Example: Extract UserId claim
                var userId = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                return Ok(new { Message = "Token is valid", UserId = userId });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name cannot be empty.");

            if (await _dBContext.Roles.AnyAsync(r => r.Name == roleName))
                return BadRequest("Role already exists.");

            var role = new Role { Name = roleName };
            _dBContext.Roles.Add(role);
            await _dBContext.SaveChangesAsync();

            return Ok(new { Message = "Role added successfully.", RoleId = role.Id });
        }

        // Add a Permission to a Role
        [HttpPost("add-permission")]
        public async Task<IActionResult> AddPermission([FromBody] RolePermissionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Permission) || request.RoleId <= 0)
                return BadRequest("Permission and RoleId are required.");

            if (!await _dBContext.Roles.AnyAsync(r => r.Id == request.RoleId))
                return NotFound("Role not found.");

            var rolePermission = new RolePermission
            {
                RoleId = request.RoleId,
                Permission = request.Permission
            };

            _dBContext.RolePermissions.Add(rolePermission);
            await _dBContext.SaveChangesAsync();

            return Ok(new { Message = "Permission added successfully.", PermissionId = rolePermission.Id });
        }

        // Get All Roles with Permissions
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _dBContext.Roles
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    Permissions = _dBContext.RolePermissions
                        .Where(rp => rp.RoleId == r.Id)
                        .Select(rp => rp.Permission)
                        .ToList()
                })
                .ToListAsync();

            return Ok(roles);
        }

        // Get Permissions for a Specific Role
        [HttpGet("get-role-permissions/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            var role = await _dBContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
                return NotFound("Role not found.");

            var permissions = await _dBContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();

            return Ok(new { Role = role.Name, Permissions = permissions });
        }
    }
}
