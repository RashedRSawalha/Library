using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using LibraryManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementApplication.Authentication
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;


        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        // Token Generation
        public string GenerateToken(int userId, string role, List<string> permissions)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("Permissions", string.Join(",", permissions))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"] ?? "60")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        // Token Validation
        public IEnumerable<Claim> ParseToken(string token, bool validateLifetime = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException("Token cannot be null or empty.");

                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];

                if (string.IsNullOrWhiteSpace(secretKey))
                    throw new InvalidOperationException("Secret key is not configured.");

                // Token handler and validation parameters
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = validateLifetime,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Retrieve and return claims from the token
                var jwtToken = (JwtSecurityToken)validatedToken;
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                return jwtToken.Claims;
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null; // Return null if validation fails
            }
        }
    }
}
