# Multi-stage Dockerfile for LegacyDataAccess.API

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY src/LegacyDataAccess.Core/*.csproj ./src/LegacyDataAccess.Core/
COPY src/LegacyDataAccess.Dapper/*.csproj ./src/LegacyDataAccess.Dapper/
COPY src/LegacyDataAccess.API/*.csproj ./src/LegacyDataAccess.API/

RUN dotnet restore src/LegacyDataAccess.API/LegacyDataAccess.API.csproj

COPY src/ ./src/

RUN dotnet publish src/LegacyDataAccess.API/LegacyDataAccess.API.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

RUN apk add --no-cache curl
RUN addgroup -g 1000 appgroup && adduser -u 1000 -G appgroup -s /bin/sh -D appuser

COPY --from=build /app/publish .
RUN chown -R appuser:appgroup /app

USER appuser

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "LegacyDataAccess.API.dll"]