//using System.IdentityModel.Tokens.Jwt;

//namespace LibraryManagementAPI.Middlewares
//{
//    public class PermissionMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public PermissionMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

//            if (!string.IsNullOrEmpty(token))
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

//                if (jwtToken != null)
//                {
//                    // Extract permissions from claims
//                    var permissions = jwtToken.Claims
//                        .Where(c => c.Type == "Permission")
//                        .Select(c => c.Value)
//                        .ToList();

//                    context.Items["Permissions"] = permissions; // Store permissions in HttpContext
//                }
//            }

//            await _next(context);
//        }
//    }
//}
