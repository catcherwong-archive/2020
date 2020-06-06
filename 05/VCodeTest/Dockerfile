FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

# 复制字体到 /usr/share/fonts 目录
COPY ./comic.ttf /usr/share/fonts/comic.ttf

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet restore "VCodeTest.sln"
WORKDIR /src/VCodeTest
RUN dotnet build "VCodeTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VCodeTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VCodeTest.dll"]