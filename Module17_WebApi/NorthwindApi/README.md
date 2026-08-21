# Northwind API

This project is the Task 1 and Task 2 solution for the Module 17 Web API assignment. It exposes a REST API over the Northwind database and includes pagination and filtering for the product list endpoint.

The API is now split into layered projects:

- `NorthwindApi.Contracts` for shared DTOs and request contracts
- `NorthwindApi.Domain` for the Northwind entities
- `NorthwindApi.Application` for services, repository abstractions, and shared results
- `NorthwindApi.Infrastructure` for SQL repositories, logging, middleware, and health checks
- `NorthwindApi` as the thin ASP.NET Core host

## What is included

- `GET /api/categories` and `GET /api/categories/{id}`
- `GET /api/products` and `GET /api/products/{id}`
- CRUD operations for both categories and products
- Pagination and category filtering for the product list endpoint
- Swagger UI for interactive testing
- API and database health checks
- Structured logging with Serilog and file output

## Endpoints

### Categories

- `GET /api/categories` - list all categories
- `GET /api/categories/{id}` - get a category by id
- `POST /api/categories` - create a category
- `PUT /api/categories/{id}` - update a category
- `DELETE /api/categories/{id}` - delete a category

### Products

- `GET /api/products` - list products with pagination and optional category filtering
- `GET /api/products/{id}` - get a product by id
- `POST /api/products` - create a product
- `PUT /api/products/{id}` - update a product
- `DELETE /api/products/{id}` - delete a product

### Product list query parameters

The products list endpoint accepts these optional query parameters:

- `pageNumber` - default `1`
- `pageSize` - default `10`
- `categoryId` - optional category filter

The response body returns the product items only, while pagination metadata is written to headers:

- `X-Total-Items`
- `X-Total-Pages`
- `X-Page-Number`
- `X-Page-Size`

## Running the API

The API uses the Northwind database connection string from `appsettings.json`.

Default connection string:

```json
"Server=localhost,1433;Database=Northwind;User Id=sa;Password=YourStrongP@ssw0rd!;Encrypt=True;TrustServerCertificate=True"
```

Run the project:

```bash
dotnet run --project NorthwindApi/NorthwindApi.csproj
```

The app uses the launch profile in `Properties/launchSettings.json` and opens Swagger by default in Development.

## Swagger

Swagger UI is enabled and configured to open on startup in Development.

Local URLs:

- HTTP: `http://localhost:5055/swagger`
- HTTPS: `https://localhost:7055/swagger`

## Health checks

The API exposes these health endpoints:

- `GET /health` - overall health
- `GET /health/api` - API availability check
- `GET /health/db` - database connectivity check

The health responses use a custom JSON writer.

## Logging

Logging is configured with Serilog and `appsettings.json`:

- console output at `Debug`
- file logging with daily rolling files
- file retention and size limits from the `LogFiles` section

Log files are written to the `logs/` folder.

## Project structure

- `Controllers/` - API controllers for categories and products
- `Services/Common/` - ASP.NET-specific result mapping helpers
- `Properties/` - launch settings for local development
- `NorthwindApi.Contracts/` - shared request and response models used by the API and consumers
- `NorthwindApi.Domain/` - domain entities shared by services and repositories
- `NorthwindApi.Application/` - service interfaces, implementations, and repository abstractions
- `NorthwindApi.Infrastructure/` - SQL Server repositories, logging, request logging middleware, and health checks

## Related consumers

Task 3 is implemented in the sibling projects:

- `NorthwindApiConsumer` - console REST client
- `NorthwindApiConsumer.Tests` - MSTest-based API consumer
- `NorthwindApi.Contracts` - shared DTOs and request contracts for the API and consumers
