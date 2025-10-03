using Microsoft.AspNetCore.Mvc;
using MGE_HEROES.Server.Services;
using MGE_HEROES.Server.Models;
using System;
using System.Threading.Tasks;

namespace MGE_HEROES.Server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthorizationController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly ConnectionManager _connectionManager;

        public AuthorizationController(AuthenticationService authService, ConnectionManager connectionManager)
        {
            _authService = authService;
            _connectionManager = connectionManager;
        }

        [HttpPost("registrate")]
        public async Task<IActionResult> Registrate([FromBody] RegisterRequest request)
        {
            Console.WriteLine($"Получен запрос регистрации: {System.Text.Json.JsonSerializer.Serialize(request)}");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Невірні дані запиту", Errors = errors });
            }

            var result = await _authService.Registrate(request.Email, request.Password, request.Username);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                result.User.Id,
                result.User.Username,
                Email = result.User.EmailHash,
                result.AccessToken
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"Получен запрос входа: {System.Text.Json.JsonSerializer.Serialize(request)}");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Невірні дані запиту", Errors = errors });
            }

            var result = await _authService.Login(request.Email, request.Password);

            if (!result.Success)
                return Unauthorized(result.Message);

            // Отправка уведомления через WebSocket
            await _connectionManager.SendMessageToUserAsync(result.User.Id.ToString(), $"Вход выполнен с другого устройства в {DateTime.UtcNow}");

            return Ok(new
            {
                result.User.Id,
                result.User.Username,
                Email = result.User.EmailHash,
                result.AccessToken
            });
        }
    }
}