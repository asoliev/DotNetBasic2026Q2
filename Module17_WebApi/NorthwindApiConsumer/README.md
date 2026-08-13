# Northwind API Consumer

This folder contains the Task 3 consumer for the Northwind Web API.

## What is included

- `NorthwindApiConsumer` - a console app that sends sample requests to the API and prints responses.
- `NorthwindApiConsumer.Tests` - an MSTest-based REST consumer that exercises the same API endpoints as automated tests.
- `appsettings.json` - base URL configuration for the console app.
- `NorthwindApiConsumer.runsettings` in `NorthwindApiConsumer.Tests` - base URL configuration for the MSTest consumer.

## Task 3 flow

The API consumer demonstrates both read and mutation calls.

Read calls:

- `GET /api/categories`
- `GET /api/products`
- `GET /api/products?pageNumber=2&pageSize=5`
- `GET /api/products?categoryId=1&pageNumber=1&pageSize=5`
- `GET /api/products/1`

Mutation calls:

- `POST /api/categories` creates a temporary category.
- `PUT /api/categories/{id}` updates that category.
- `POST /api/products` creates a temporary product.
- `PUT /api/products/{id}` updates that product.
- `DELETE /api/products/{id}` removes the temporary product.
- `DELETE /api/categories/{id}` removes the temporary category.

## Base URL configuration

The console app reads the API base URL from `appsettings.json`.

The MSTest project reads the API base URL from `NorthwindApiConsumer.Tests/NorthwindApiConsumer.runsettings`.

Default value:

- `http://127.0.0.1:5055`

If you want to point either consumer at another API instance, change the base URL in the corresponding file.

## Run examples

Console consumer:

```bash
dotnet run --project NorthwindApiConsumer/NorthwindApiConsumer.csproj
```

MSTest consumer:

```bash
dotnet test NorthwindApiConsumer.Tests/NorthwindApiConsumer.Tests.csproj --settings NorthwindApiConsumer.Tests/NorthwindApiConsumer.runsettings
```

If you prefer to keep the test configuration fully explicit, pass the same settings file every time you run the MSTest project. The console app does not need an extra command-line setting because it reads `appsettings.json` directly.
