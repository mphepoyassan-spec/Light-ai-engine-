FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["backend_csharp/backend_csharp.csproj", "backend_csharp/"]
RUN dotnet restore "backend_csharp/backend_csharp.csproj"
COPY . .
WORKDIR "/src/backend_csharp"
RUN dotnet build "backend_csharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend_csharp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy frontend assets
COPY frontend/ ./frontend/
EXPOSE 8000
ENTRYPOINT ["dotnet", "backend_csharp.dll", "--urls", "http://0.0.0.0:8000"]
