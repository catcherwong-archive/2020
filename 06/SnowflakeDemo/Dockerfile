FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet restore "SnowflakeDemo.sln" -s https://api.nuget.org/v3/index.json
WORKDIR /src/SnowflakeDemo
RUN dotnet build "SnowflakeDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SnowflakeDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SnowflakeDemo.dll"]