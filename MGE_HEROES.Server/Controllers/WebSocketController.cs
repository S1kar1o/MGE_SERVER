using Microsoft.AspNetCore.Mvc;
using MGE_HEROES.Server.Servises;

namespace MGE_HEROES.Server.Controllers
{
    [ApiController]
    [Route("ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly ConnectionManager _connectionManager;
        private readonly MessageProcessor _messageProcessor;

        public WebSocketController(ConnectionManager connectionManager, MessageProcessor messageProcessor)
        {
            _connectionManager = connectionManager;
            _messageProcessor = messageProcessor;
        }

        [HttpGet]
        public async Task Get()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var userId = HttpContext.Request.Query["userId"].ToString();
            Console.WriteLine($"WebSocket запрос с userId: {userId}");
            await _connectionManager.HandleWebSocketAsync(webSocket, userId, _messageProcessor);
        }
    }
}