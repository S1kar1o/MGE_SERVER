using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MGE_HEROES.Server.Services;
using MGE_HEROES.Server.Servises;

namespace MGE_HEROES.Server
{
    public class GameConnection
    {
        public string Id { get; }
        public WebSocket Socket { get; }
        public User User { get; }

        public GameConnection(string id, WebSocket socket, User user)
        {
            Id = id;
            Socket = socket;
            User = user;
        }
    }

    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<string, GameConnection> _connections = new();
        private readonly MessageProccesor _messageProcessor;

        public ConnectionManager(MessageProccesor messageProcessor)
        {
            _messageProcessor = messageProcessor;
        }

        public string Add(WebSocket socket, User user = null)
        {
            var id = Guid.NewGuid().ToString();
            user ??= new User(); // Пустой пользователь, если не передан
            var connection = new GameConnection(id, socket, user);
            _connections.TryAdd(id, connection);
            Console.WriteLine($"Клієнт підключився: {id}");
            return id;
        }

        public void Remove(string id)
        {
            if (_connections.TryRemove(id, out _))
            {
                Console.WriteLine($"Клієнт відключений: {id}");
            }
        }

        public GameConnection? Get(string id)
        {
            _connections.TryGetValue(id, out var connection);
            return connection;
        }

        public IEnumerable<GameConnection> GetAll()
        {
            return _connections.Values;
        }

        public async Task SendMessageToUserAsync(string connectionId, string message)
        {
            if (_connections.TryGetValue(connectionId, out var connection) && connection.Socket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await connection.Socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task HandleWebSocketAsync(WebSocket socket, string userId, MessageProccesor messageProcessor = null)
        {
            User user = new User();
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var parsedUserId))
            {
                user.Id = parsedUserId;
            }
            else
            {
                Console.WriteLine($"Невалидный userId: {userId}. Используется пустой User.");
            }

            var connectionId = Add(socket, user);

            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"[{connectionId}] Отримав: {message}");
                        _messageProcessor?.StartProcessingMessage(message); // Обработка сообщения
                        await SendMessageToUserAsync(connectionId, $"Сервер отримав: {message}");
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Remove(connectionId);
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в HandleWebSocketAsync: {ex.Message}");
                    break;
                }
            }
        }
    }
}