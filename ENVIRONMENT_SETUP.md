# Environment Setup Guide

## Protecting Sensitive Information

This project uses environment variables to manage sensitive credentials like database passwords. Never commit sensitive information to version control.

## PostgreSQL Container Credentials

### Default Development Credentials (Change in Production!)

The `.env.example` file contains the template for environment variables:
- **POSTGRES_USER**: postgres
- **POSTGRES_PASSWORD**: postgres (⚠️ Change for production)
- **POSTGRES_DB**: postgres
- **REDIS_HOST**: localhost
- **REDIS_PORT**: 6379

## Setup Instructions

1. **Create a local `.env` file** (not committed to git):
   ```bash
   cp .env.example .env
   ```

2. **Edit `.env` with your secure credentials**:
   ```
   POSTGRES_USER=your_username
   POSTGRES_PASSWORD=your_secure_password
   POSTGRES_DB=your_database
   REDIS_HOST=localhost
   REDIS_PORT=6379
   ```

3. **For Docker Compose**, create `docker-compose.override.yml`:
   ```bash
   cp docker-compose.example.yml docker-compose.override.yml
   ```

4. **Update `docker-compose.override.yml`** with your credentials

## Configuration Files

- **appsettings.json.example** - Template for application settings (includes defaults)
- **docker-compose.example.yml** - Template for Docker services with environment variable placeholders
- **.env.example** - Template for environment variables

## Using Environment Variables

In your application code, read from environment variables:

```csharp
var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
```

Or use `appsettings.json` with variable substitution in Docker:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=postgres;Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};"
}
```

## Security Best Practices

✅ DO:
- Use environment variables for secrets
- Create `.env.example` templates with placeholders
- Use `.gitignore` to exclude sensitive files
- Use different credentials for dev/staging/production
- Rotate passwords regularly in production

❌ DON'T:
- Commit `.env` files to git
- Hardcode passwords in source code
- Use default credentials in production
- Share credentials in chat/email

## Files Excluded from Git

The `.gitignore` file excludes:
- `.env` (local environment variables)
- `appsettings.*.json` (environment-specific settings)
- `docker-compose.override.yml` (local overrides)
- `secrets.json` (user secrets)
