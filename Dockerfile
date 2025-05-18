# Используем .NET 8 SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проект
COPY . .

# Восстанавливаем зависимости
RUN dotnet restore

# Сборка приложения
RUN dotnet publish -c Release -o /app

# Используем .NET 8 Runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

# Открываем порт (если надо)
EXPOSE 80

# Запуск приложения
ENTRYPOINT ["dotnet", "StudentClinicMIS.dll"]
