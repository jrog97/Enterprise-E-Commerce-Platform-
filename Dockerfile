# =========================
# Build stage
# =========================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["ECommerce.Api/ECommerce.Api.csproj", "ECommerce.Api/"]
COPY ["ECommerce.Application/ECommerce.Application.csproj", "ECommerce.Application/"]
COPY ["ECommerce.Domain/ECommerce.Domain.csproj", "ECommerce.Domain/"]
COPY ["ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "ECommerce.Infrastructure/"]

RUN dotnet restore "ECommerce.Api/ECommerce.Api.csproj"

COPY . .

WORKDIR "/src/ECommerce.Api"

RUN dotnet publish \
    "ECommerce.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# =========================
# Runtime stage
# =========================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

RUN apt-get update && apt-get install -y \
    libgssapi-krb5-2 \
    libpq5 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "ECommerce.Api.dll"]