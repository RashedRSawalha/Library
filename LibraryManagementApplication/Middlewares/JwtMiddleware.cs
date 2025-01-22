using LibraryManagementApplication.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace LibraryManagementAPI.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenService _tokenService;

        public JwtMiddleware(RequestDelegate next, TokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var claims = _tokenService.ParseToken(token);

                if (claims != null)
                {
                    // Extract role from claims
                    var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Extract permissions from claims
                    var permissions = claims
                        .Where(c => c.Type == "Permissions")
                        .Select(c => c.Value.Split(','))
                        .FirstOrDefault()?
                        .ToList();

                    // Store role and permissions in HttpContext
                    if (!string.IsNullOrEmpty(role))
                    {
                        context.Items["Role"] = role;
                    }

                    if (permissions != null)
                    {
                        context.Items["Permissions"] = permissions;
                    }
                }
            }

            await _next(context);
        }




        private static Task RespondUnauthorizedAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync($"{{\"message\":\"{message}\"}}");
        }
    }
}
