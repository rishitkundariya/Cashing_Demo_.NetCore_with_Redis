# CachingDemo - Docker Setup

This project is fully containerized with Docker and Docker Compose. All services (PostgreSQL, Redis, and the ASP.NET Core application) run in containers.

## Prerequisites

- Docker Desktop (Windows) or Docker Engine (Linux/Mac)
- Docker Compose (usually included with Docker Desktop)

## Quick Start

### Start All Services

```bash
docker-compose up -d
```

This command will:
1. Create a bridge network for service communication
2. Start PostgreSQL database (port 5432)
3. Start Redis cache (port 6379)
4. Build and start the CachingDemo application (port 5151)

### Access the Application

Once all containers are running, access the application at:
- **Application**: http://localhost:5151
- **OpenAPI/Swagger**: http://localhost:5151/openapi/v1.json

### Stop All Services

```bash
docker-compose down
```

To also remove volumes and start fresh:
```bash
docker-compose down -v
```

## Services

### PostgreSQL Database
- **Port**: 5432
- **Username**: postgres
- **Password**: postgres
- **Database**: postgres
- **Container Name**: postgres_db
- **Volume**: postgres_data

### Redis Cache
- **Port**: 6379
- **Container Name**: redis_cache
- **Volume**: redis_data
- **Persistence**: Enabled (RDB snapshots)

### CachingDemo Application
- **Port**: 5151 (exposed from 8080 internal)
- **Container Name**: caching-demo-app
- **Environment**: Development
- **Features**:
  - In-memory caching for departments
  - Distributed caching (Redis) for employees
  - EF Core with PostgreSQL
  - Automatic database migrations

## Configuration

Connection strings are configured via Docker Compose environment variables:

- **PostgreSQL**: `Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres;`
- **Redis**: `redis:6379`

These use Docker's internal DNS where service names resolve to container IPs.

## Docker Compose Services Communication

Services communicate using their service names as hostnames:
- App → PostgreSQL: `postgres:5432`
- App → Redis: `redis:6379`

## Building the Docker Image Manually

```bash
docker build -t cachingdemo-app . -f Dockerfile
```

## Viewing Logs

### All services
```bash
docker-compose logs -f
```

### Specific service
```bash
docker-compose logs -f app
docker-compose logs -f postgres
docker-compose logs -f redis
```

### Container logs directly
```bash
docker logs caching-demo-app -f
docker logs postgres_db
docker logs redis_cache
```

## Health Status

Check container health:
```bash
docker ps
```

All containers should show as "Up" or "Healthy" status.

## Volumes

Data persistence:
- **postgres_data**: PostgreSQL data and configuration
- **redis_data**: Redis snapshots and append-only file

To remove all data and start fresh:
```bash
docker-compose down -v
docker-compose up -d
```

## Network

All containers are connected via a bridge network named `caching-demo-network`, allowing them to communicate using service names.

## Development

For local development without Docker:
```bash
dotnet run
```

Make sure to update connection strings in `appsettings.json` to use `localhost` instead of service names.

## Docker Files

- **Dockerfile**: Multi-stage build for optimized image size
  - Build stage: Compiles the application
  - Runtime stage: Minimal runtime environment
- **docker-compose.yml**: Orchestrates all services
- **.dockerignore**: Excludes unnecessary files from Docker build context

## Troubleshooting

### Application not connecting to PostgreSQL
- Ensure PostgreSQL container is healthy: `docker logs postgres_db`
- Check network connectivity: `docker network inspect cachingdemo_caching-demo-network`

### Redis connection issues
- Verify Redis is running: `docker ps | grep redis_cache`
- Check Redis logs: `docker logs redis_cache`

### Port conflicts
- If port 5151, 5432, or 6379 are in use, modify `docker-compose.yml`
- Change port mappings like: `"5152:8080"` for a different host port

### Clear Docker resources
```bash
docker-compose down -v
docker system prune -f
```
