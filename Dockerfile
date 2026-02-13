FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/WorshipManager.Core/WorshipManager.Core.csproj", "src/WorshipManager.Core/"]
COPY ["src/WorshipManager.Application/WorshipManager.Application.csproj", "src/WorshipManager.Application/"]
COPY ["src/WorshipManager.Infrastructure/WorshipManager.Infrastructure.csproj", "src/WorshipManager.Infrastructure/"]
COPY ["src/WorshipManager.Api/WorshipManager.Api.csproj", "src/WorshipManager.Api/"]
RUN dotnet restore "src/WorshipManager.Api/WorshipManager.Api.csproj"
COPY . .
RUN dotnet publish "src/WorshipManager.Api/WorshipManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]
