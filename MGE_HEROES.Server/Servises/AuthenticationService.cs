using Supabase;
using MGE_HEROES.Server;
using System;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MGE_HEROES.Server.Services
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public User? User { get; set; }
        public string? AccessToken { get; set; }
    }
    public class AuthenticationService
    {
        private readonly Client _supabase;

        public AuthenticationService(GameDbContext db)
        {
            _supabase = db.GetClient();
        }
        public async Task<AuthResult> Login(string email, string password)
        {
            try
            {
                var users = await _supabase.From<User>()
                    .Where(u => u.EmailHash == email) // або u.Email, якщо не хешуєте
                    .Get();

                var user = users.Models.FirstOrDefault();
                if (user == null)
                    return new AuthResult { Success = false, Message = "Користувача не знайдено" };

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return new AuthResult { Success = false, Message = "Невірний пароль" };

                // Генерація токена (наприклад, JWT)
                string accessToken = GenerateJwtToken(user); // Реалізуйте цей метод

                return new AuthResult
                {
                    Success = true,
                    User = user,
                    AccessToken = accessToken
                };
            }
            catch (Exception ex)
            {
                // Використовуйте ILogger замість Console.WriteLine
                return new AuthResult { Success = false, Message = $"Помилка при логіні: {ex.Message}" };
            }
        }

        public async Task<AuthResult> Registrate(string email, string password, string username)
        {
            try
            {
                var existingUser = await _supabase.From<User>()
                    .Where(u => u.EmailHash == email)
                    .Get();

                if (existingUser.Models.Any())
                    return new AuthResult { Success = false, Message = "Користувач з таким email вже існує" };

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    EmailHash = email, // або хешуйте email, якщо це необхідно
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                };

                await _supabase.From<User>().Insert(newUser);

                string accessToken = GenerateJwtToken(newUser); // Реалізуйте цей метод

                return new AuthResult
                {
                    Success = true,
                    User = newUser,
                    AccessToken = accessToken
                };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = $"Помилка при реєстрації: {ex.Message}" };
            }
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.EmailHash)
    };

            // Використовуйте ключ довжиною щонайменше 32 байти
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this_is_a_very_long_secret_key_32_bytes!")); // 32+ байти
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
