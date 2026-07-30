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

## Running Build and Tests

From `Module13_ADO_NET`:

```bash
dotnet build Module13_ADO_NET.slnx
```

Tests require a real SQL Server connection string in `ADO_NET_TEST_CONNECTION_STRING`.

Example:

```bash
export ADO_NET_TEST_CONNECTION_STRING="Server=localhost,1433;Database=AdoNetModule13;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
dotnet test --project AdoNetLibrary.Tests/AdoNetLibrary.Tests.csproj
```

## Test Behavior Without Connection String

Tests are intentionally strict now.
If `ADO_NET_TEST_CONNECTION_STRING` is not set, tests fail explicitly instead of passing silently.
