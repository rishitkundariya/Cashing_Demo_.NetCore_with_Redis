#!/bin/bash
# Entrypoint script for Docker container
# Waits for external services (PostgreSQL, Redis) to be ready before starting the application

set -e

echo "🚀 CachingDemo Application Starting..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# PostgreSQL Connection Details
POSTGRES_HOST="${POSTGRES_HOST:-postgres}"
POSTGRES_PORT="${POSTGRES_PORT:-5432}"

# Redis Connection Details
REDIS_HOST="${REDIS_HOST:-redis}"
REDIS_PORT="${REDIS_PORT:-6379}"

# Wait Parameters
MAX_RETRIES=30
RETRY_INTERVAL=2

echo "⏳ Waiting for PostgreSQL ($POSTGRES_HOST:$POSTGRES_PORT) to be ready..."
RETRY_COUNT=0
while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if timeout 2 bash -c "echo > /dev/tcp/$POSTGRES_HOST/$POSTGRES_PORT" 2>/dev/null; then
        echo "✅ PostgreSQL is ready!"
        break
    fi
    
    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -lt $MAX_RETRIES ]; then
        echo "⏳ PostgreSQL not ready yet... (Attempt $RETRY_COUNT/$MAX_RETRIES)"
        sleep $RETRY_INTERVAL
    fi
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "❌ PostgreSQL failed to start within expected time"
    exit 1
fi

echo "⏳ Waiting for Redis ($REDIS_HOST:$REDIS_PORT) to be ready..."
RETRY_COUNT=0
while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if timeout 2 bash -c "echo > /dev/tcp/$REDIS_HOST/$REDIS_PORT" 2>/dev/null; then
        echo "✅ Redis is ready!"
        break
    fi
    
    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -lt $MAX_RETRIES ]; then
        echo "⏳ Redis not ready yet... (Attempt $RETRY_COUNT/$MAX_RETRIES)"
        sleep $RETRY_INTERVAL
    fi
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "⚠️  Redis failed to start within expected time (continuing anyway)"
fi

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ All services are ready!"
echo "🚀 Starting application..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Execute the application
exec "$@"
