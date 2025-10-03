using MGE_HEROES.Server;
using MGE_HEROES.Server.Services;
using MGE_HEROES.Server.Servises;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Получение конфигурации для Supabase

var url = "https://qquyntuptdauvjudnxkc.supabase.co" /*builder.Configuration["Supabase:Url"]*/;
var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFxdXludHVwdGRhdXZqdWRueGtjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTkyMzAyMTgsImV4cCI6MjA3NDgwNjIxOH0.OoDTB_a9vcvjrJgEwLOMHGHvi7vH8IM832quiT1pIlk" /*builder.Configuration["Supabase:Key"]*/;

// Регистрация сервисов
builder.Services.AddSingleton(new GameDbContext(url, key));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // Для совместимости с Unity JsonUtility
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // Для читаемости в логах
});
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddSingleton<MessageProccesor>(); // Регистрация MessageProcessor

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

var app = builder.Build();

// Конфигурация HTTP pipeline
app.UseRouting();
app.UseCors("AllowAll");
app.UseWebSockets();

if (app.Environment.IsDevelopment())
{
    // Можно добавить Swagger для отладки
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Закомментировано для локальной разработки
app.UseAuthorization();
app.MapControllers();

app.Run();