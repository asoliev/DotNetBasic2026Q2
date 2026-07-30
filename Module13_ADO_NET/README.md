# Module13 ADO.NET

This module contains:
- `AdoNetLibrary`: ADO.NET data access library
- `AdoNetLibrary.Tests`: integration tests for CRUD, filtering, and bulk delete behavior

## Implemented Requirements

1. Product CRUD operations
2. Order CRUD operations
3. Fetch all products
4. Fetch orders by filters (month, year, status, product) via stored procedure
5. Bulk delete orders by same filters using a transaction

## Database Setup

Use SQL scripts in order:

1. `AdoNetLibrary/Scripts/001_create_schema.sql`
2. `AdoNetLibrary/Scripts/002_create_procedures.sql`

### Quick SQL Server Setup on macOS (Docker Compose)

From `Module13_ADO_NET`:

```bash
docker compose up -d
```

One-command bootstrap (start DB, create database, apply scripts):

```bash
./scripts/bootstrap-db.sh
```

One-command real DB test run (bootstrap + integration tests):

```bash
./scripts/test-with-db.sh
```

Optional variables:

```bash
MSSQL_SA_PASSWORD="Your_password123" MODULE13_DB_NAME="AdoNetModule13" ./scripts/bootstrap-db.sh
MSSQL_SA_PASSWORD="Your_password123" MODULE13_DB_NAME="AdoNetModule13" ./scripts/test-with-db.sh
```

Wait for SQL Server health status:

```bash
docker compose ps
```

Create database:

```bash
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
	-S localhost -U sa -P "Your_password123" -C \
	-Q "IF DB_ID('AdoNetModule13') IS NULL CREATE DATABASE AdoNetModule13;"
```

Copy and run scripts:

```bash
docker cp AdoNetLibrary/Scripts/001_create_schema.sql module13_sqlserver:/tmp/001_create_schema.sql
docker cp AdoNetLibrary/Scripts/002_create_procedures.sql module13_sqlserver:/tmp/002_create_procedures.sql

docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
	-S localhost -U sa -P "Your_password123" -C -d AdoNetModule13 \
	-i /tmp/001_create_schema.sql

docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
	-S localhost -U sa -P "Your_password123" -C -d AdoNetModule13 \
	-i /tmp/002_create_procedures.sql
```

## Running Build and Tests

From `Module13_ADO_NET`:

```bash
dotnet build Module13_ADO_NET.slnx
```

Tests require a real SQL Server connection string in `ADO_NET_TEST_CONNECTION_STRING`.

Example:

```bash
export ADO_NET_TEST_CONNECTION_STRING="Server=localhost,1433;Database=AdoNetModule13;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
dotnet test AdoNetLibrary.Tests/AdoNetLibrary.Tests.csproj
```

To stop DB:

```bash
docker compose down
```

## Test Behavior Without Connection String

Tests currently return early when `ADO_NET_TEST_CONNECTION_STRING` is not set.
That means they can pass without executing real DB calls.
