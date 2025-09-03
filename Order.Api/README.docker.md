# Order API Docker Setup

This README contains instructions on how to build and run the Order API using Docker.

## Prerequisites

- Docker
- Docker Compose
- .NET 9.0 SDK (for local development)

## Running with Docker Compose

### Using the provided docker-compose.order.yml

The easiest way to run the Order API with its PostgreSQL database is using Docker Compose:

```bash
# Navigate to the solution root folder where docker-compose.order.yml is located
docker-compose -f docker-compose.order.yml up -d
```

This will:
1. Build the Order API Docker image
2. Start a PostgreSQL database container configured for the Order API
3. Start the Order API container connected to the database
4. Map the Order API to ports 5005 (HTTP) and 5006 (HTTPS)

### Accessing the API

Once running, the API will be available at:
- HTTP: http://localhost:5005
- HTTPS: https://localhost:5006

## Building the Docker Image Manually

If you prefer to build and run the Docker image manually:

```bash
# Build the image
docker build -f Order.Api/Dockerfile -t order-api .

# Run the container
docker run -d -p 5005:80 -p 5006:443 \
  -e ConnectionStrings__OrderConnection="Host=your_postgres_host;Database=OrderDb;Username=orderadmin;Password=order123" \
  --name order-api \
  order-api
```

## HTTPS Setup

For HTTPS to work correctly, you need to have a valid certificate. The docker-compose file is configured to use a certificate from your local machine. To generate a development certificate:

```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p password
dotnet dev-certs https --trust
```

## Database Migrations

The first time you run the application, you may need to apply database migrations. You can do this by:

1. Adding code to Program.cs to automatically apply migrations at startup, or
2. Running migrations manually before starting the container:

```bash
dotnet ef database update --project Order.Infrastructures --startup-project Order.Api
```

## Environment Variables

The following environment variables can be modified:

- `ASPNETCORE_ENVIRONMENT`: Set to Development, Staging, or Production
- `ConnectionStrings__OrderConnection`: PostgreSQL connection string
- `ASPNETCORE_URLS`: HTTP/HTTPS endpoints
- `ASPNETCORE_Kestrel__Certificates__Default__Password`: Certificate password
- `ASPNETCORE_Kestrel__Certificates__Default__Path`: Certificate path

## Volumes

The PostgreSQL data is persisted using a Docker volume named `postgres_order_data`.