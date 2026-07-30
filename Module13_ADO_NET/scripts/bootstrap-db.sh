#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

CONTAINER_NAME="module13_sqlserver"
DB_NAME="${MODULE13_DB_NAME:-AdoNetModule13}"
SA_PASSWORD="${MSSQL_SA_PASSWORD:-Your_password123}"

echo "Starting SQL Server container with Docker Compose..."
docker compose up -d --wait --wait-timeout 180

echo "Creating database '$DB_NAME' if it does not exist..."
docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C \
  -Q "IF DB_ID('$DB_NAME') IS NULL CREATE DATABASE [$DB_NAME];"

echo "Copying SQL scripts into container..."
docker cp AdoNetLibrary/Scripts/001_create_schema.sql "$CONTAINER_NAME:/tmp/001_create_schema.sql"
docker cp AdoNetLibrary/Scripts/002_create_procedures.sql "$CONTAINER_NAME:/tmp/002_create_procedures.sql"

echo "Applying schema script..."
docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" \
  -i /tmp/001_create_schema.sql

echo "Applying procedures script..."
docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$SA_PASSWORD" -C -d "$DB_NAME" \
  -i /tmp/002_create_procedures.sql

echo
echo "Database bootstrap completed."
echo "Use this connection string for tests:"
echo "Server=localhost,1433;Database=$DB_NAME;User Id=sa;Password=$SA_PASSWORD;TrustServerCertificate=True;"
