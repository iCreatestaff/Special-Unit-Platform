using Microsoft.IdentityModel.Tokens;
using sp_backend.DTO; // Ensure LoginRequest or similar DTO is defined here if needed
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherApi; // Ensure this is correct if used elsewhere

namespace sp_backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _jwtKey = config["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is missing in configuration");
            _issuer = config["Jwt:Issuer"] ?? "sp_backend";
            _audience = config["Jwt:Audience"] ?? "sp_frontend";

            // Validate JWT key length (must be at least 16 characters for HS256)
            if (_jwtKey.Length < 16)
            {
                throw new ArgumentException("JWT Key must be at least 16 characters long for HmacSha256");
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        public string GenerateJwtToken(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account), "Account cannot be null");
            }

            var claims = new[]
            {
        new Claim("nameid", account.Id.ToString()), // Use "nameid" instead of ClaimTypes.NameIdentifier
        new Claim(ClaimTypes.Name, account.Username ?? string.Empty),
        new Claim(ClaimTypes.Role, account.Role ?? string.Empty)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Generated token for {account.Username} (ID: {account.Id}) with role {account.Role}: {tokenString}");
            return tokenString;
        }

        public string Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Authentication failed: Username or password is empty");
                return null;
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Username == username);
            if (account == null)
            {
                Console.WriteLine($"Authentication failed: No account found for username '{username}'");
                return null;
            }

            if (!VerifyPassword(password, account.PasswordHash))
            {
                Console.WriteLine($"Authentication failed: Invalid password for username '{username}'");
                return null;
            }

            return GenerateJwtToken(account);
        }
    }
}