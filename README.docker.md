# Microservices Docker Setup Guide

This guide provides instructions for setting up and running the entire microservices architecture using Docker.

## Architecture Overview

The system consists of the following services:

1. **AuthServer API**: Authentication and authorization service
2. **Customer API**: Customer management service  
3. **Order API**: Order management service
4. **Product API**: Product catalog service
5. **PostgreSQL databases**: Separate database for each service
6. **pgAdmin**: Web-based PostgreSQL administration tool

## Prerequisites

- Docker and Docker Compose
- .NET 9.0 SDK (for local development and migrations)
- PowerShell (for running the helper scripts)

## Quick Start

### Using the Helper Script

The easiest way to get started is to use the provided PowerShell script:
./run-services.ps1
This interactive script provides options to:
- Build and start all services
- Start services (without rebuilding)
- Stop all services
- View running services
- View service logs
- Restart specific services
- Connect to databases
- Execute database migrations

### Manual Commands

If you prefer to use Docker Compose directly:
# Build and start all services
docker-compose build
docker-compose up -d

# Start without rebuilding
docker-compose up -d

# Stop all services
docker-compose down

# View logs for all services
docker-compose logs

# View logs for a specific service
docker-compose logs auhtserver.api
## Service Access

Once the services are running, they can be accessed at:

| Service | HTTP URL | HTTPS URL |
|---------|----------|-----------|
| Auth Server | http://localhost:8080 | https://localhost:8081 |
| Customer API | http://localhost:8082 | https://localhost:8083 |
| Order API | http://localhost:8084 | https://localhost:8085 |
| Product API | http://localhost:8086 | https://localhost:8087 |
| pgAdmin | http://localhost:5050 | N/A |

## Database Access

### Using pgAdmin

1. Access pgAdmin at http://localhost:5050
2. Login with:
   - Email: admin@authserver.com
   - Password: admin123
3. Add server connections:

| Name | Host | Port | Database | Username | Password |
|------|------|------|----------|----------|----------|
| AuthServer | authserver-postgres | 5432 | AuthServerDb | authserver | authserver123 |
| Customer | customer-postgres | 5432 | CustomerDb | customeradmin | customer123 |
| Order | order-postgres | 5432 | OrderDb | orderadmin | order123 |
| Product | product-postgres | 5432 | ProductDb | productadmin | product123 |

> Note: When connecting from inside Docker containers, use the container names as hosts. When connecting from your local machine, use localhost with the mapped ports (5433-5436).

### Direct Database Connection

You can also connect directly to the databases from your local machine:

| Database | Host | Port | Database | Username | Password |
|----------|------|------|----------|----------|----------|
| AuthServer | localhost | 5433 | AuthServerDb | authserver | authserver123 |
| Customer | localhost | 5434 | CustomerDb | customeradmin | customer123 |
| Order | localhost | 5435 | OrderDb | orderadmin | order123 |
| Product | localhost | 5436 | ProductDb | productadmin | product123 |

## Database Migrations

To run database migrations, you can use the helper script (option 9) or run them manually:
# AuthServer migrations
dotnet ef database update --project Authserver.Infrastructure --startup-project AuhtServer.Api

# Customer migrations
dotnet ef database update --project Customer.Infrastructure --startup-project Customer.Api

# Order migrations
dotnet ef database update --project Order.Infrastructures --startup-project Order.Api

# Product migrations
dotnet ef database update --project Product.Infrastructure --startup-project Product.Api
## Troubleshooting

### Port Conflicts

If you encounter port conflicts, you can modify the port mappings in the docker-compose.yml file.

### Database Connection Issues

If services cannot connect to their databases:

1. Ensure all containers are running: `docker-compose ps`
2. Check the logs for connection errors: `docker-compose logs [service_name]`
3. Verify the connection strings in the docker-compose.yml file

### Container Start Failures

If containers fail to start:

1. Check if the ports are already in use
2. Examine the logs: `docker-compose logs [service_name]`
3. Try rebuilding: `docker-compose build --no-cache [service_name]`

## Data Persistence

All PostgreSQL data is persisted in Docker volumes:
- postgres_authserver_data
- postgres_customer_data
- postgres_order_data
- postgres_product_data

These volumes ensure your data remains intact between container restarts.

## Customization

### Environment Variables

You can customize various settings in the docker-compose.yml file:

- Database credentials
- JWT settings
- Port mappings
- Volume configurations

### Adding New Services

To add a new service:

1. Create a Dockerfile for the service
2. Add the service to the docker-compose.yml file
3. Configure appropriate networks and dependencies
4. Update the helper scripts if needed