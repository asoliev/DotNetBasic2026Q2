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

The schema is organized as a SQL project in `AdoNetDb/Module13_ADO_NET.sqlproj`.
The bootstrap script applies the SQL project sources into SQL Server.

Build the SQL project with:

```bash
dotnet build AdoNetDb/Module13_ADO_NET.sqlproj
```

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
docker cp AdoNetDb/Tables/Products.sql module13_sqlserver:/tmp/Products.sql
docker cp AdoNetDb/Tables/Orders.sql module13_sqlserver:/tmp/Orders.sql
docker cp AdoNetDb/StoredProcedures/usp_GetOrders.sql module13_sqlserver:/tmp/usp_GetOrders.sql
docker cp AdoNetDb/StoredProcedures/usp_DeleteOrders.sql module13_sqlserver:/tmp/usp_DeleteOrders.sql

docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
	-S localhost -U sa -P "Your_password123" -C -d AdoNetModule13 \
	-i /tmp/Products.sql

docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
	-S localhost -U sa -P "Your_password123" -C -d AdoNetModule13 \
	-i /tmp/Orders.sql

docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
	-S localhost -U sa -P "Your_password123" -C -d AdoNetModule13 \
	-i /tmp/usp_GetOrders.sql

docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
	-S localhost -U sa -P "Your_password123" -C -d AdoNetModule13 \
	-i /tmp/usp_DeleteOrders.sql
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
