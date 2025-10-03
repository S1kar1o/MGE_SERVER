# Базовий образ для виконання (ASP.NET Core runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Render мапить внутрішній порт (10000) на 443, але вкажіть 8080 для явності
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Образ для білду
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Копіюємо .csproj і відновлюємо залежності
COPY ["MGE_HEROES.Server/MGE_HEROES.Server.csproj", "."]
RUN dotnet restore "MGE_HEROES.Server.csproj"
# Копіюємо решту файлів і публікуємо
COPY . .
RUN dotnet publish "MGE_HEROES.Server/MGE_HEROES.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Фінальний образ
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MGE_HEROES.Server.dll"]