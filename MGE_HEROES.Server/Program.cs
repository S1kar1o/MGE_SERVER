using MGE_HEROES.Server;
using MGE_HEROES.Server.Services;
using MGE_HEROES.Server.Servises; // Виправте опечатку: "Servises" -> "Services" (якщо це помилка)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Получение конфигурации для Supabase
var url = builder.Configuration["SUPABASE_URL"] ?? throw new InvalidOperationException("SUPABASE_URL is not set");
var key = builder.Configuration["SUPABASE_ANON_KEY"] ?? throw new InvalidOperationException("SUPABASE_ANON_KEY is not set");

// Регистрация сервисов
builder.Services.AddSingleton(new GameDbContext(url, key));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // Для совместимости с Unity JsonUtility
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // Для читаемости в логах
});
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddSingleton<MessageProccesor>(); // Виправте опечатку, якщо це "MessageProcessor"

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Налаштування Kestrel для Render
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

// Конфигурация HTTP pipeline
app.UseRouting();
app.UseCors("AllowAll");
app.UseWebSockets();

// app.UseHttpsRedirection(); // Render обробляє HTTPS автоматично
app.UseAuthorization();
app.MapControllers();

// Додаємо health check ендпоінт
app.MapGet("/health", () => Results.Ok("Server is running"));

// Налаштування WebSocket
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var connectionManager = context.RequestServices.GetRequiredService<ConnectionManager>();
        await connectionManager.HandleWebSocketAsync(webSocket, context.Request.Query["userId"]);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();