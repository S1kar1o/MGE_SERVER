using MGE_HEROES.Server;
using MGE_HEROES.Server.Services;
using MGE_HEROES.Server.Servises; // �������� ��������: "Servises" -> "Services" (���� �� �������)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������������ ��� Supabase
var url = builder.Configuration["SUPABASE_URL"] ?? throw new InvalidOperationException("SUPABASE_URL is not set");
var key = builder.Configuration["SUPABASE_ANON_KEY"] ?? throw new InvalidOperationException("SUPABASE_ANON_KEY is not set");

// ����������� ��������
builder.Services.AddSingleton(new GameDbContext(url, key));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // ��� ������������� � Unity JsonUtility
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // ��� ���������� � �����
});
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddSingleton<MessageProccesor>(); // �������� ��������, ���� �� "MessageProcessor"

// ��������� CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// ������������ Kestrel ��� Render
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

// ������������ HTTP pipeline
app.UseRouting();
app.UseCors("AllowAll");
app.UseWebSockets();

// app.UseHttpsRedirection(); // Render �������� HTTPS �����������
app.UseAuthorization();
app.MapControllers();

// ������ health check �������
app.MapGet("/health", () => Results.Ok("Server is running"));

// ������������ WebSocket
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