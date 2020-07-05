FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ServerlessNetApp.sln"
WORKDIR /src/GwApi
RUN dotnet build "GwApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GwApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 按需放开
#RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "GwApi.dll"]
