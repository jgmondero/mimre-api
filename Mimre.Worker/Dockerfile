FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mimre.Domain/Mimre.Domain.csproj", "Mimre.Domain/"]
COPY ["Mimre.Application/Mimre.Application.csproj", "Mimre.Application/"]
COPY ["Mimre.Infrastructure/Mimre.Infrastructure.csproj", "Mimre.Infrastructure/"]
COPY ["Mimre.Worker/Mimre.Worker.csproj", "Mimre.Worker/"]
RUN dotnet restore "Mimre.Worker/Mimre.Worker.csproj"

COPY ["Mimre.Domain/", "Mimre.Domain/"]
COPY ["Mimre.Application/", "Mimre.Application/"]
COPY ["Mimre.Infrastructure/", "Mimre.Infrastructure/"]
COPY ["Mimre.Worker/", "Mimre.Worker/"]

WORKDIR "/src/Mimre.Worker"
RUN dotnet build "Mimre.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mimre.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install SkiaSharp native dependencies
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Mimre.Worker.dll"]