# Retail Analytics Platform (Java + .NET + PostgreSQL)

A small microservice system:

| Service | Tech | Port | Role |
|---|---|---|---|
| `ingestion-service-java` | Java 17, Spring Boot 3 | 8080 | Accepts sales via REST and simulates a live sales feed |
| `reporting-api-dotnet` | .NET 8, ASP.NET Core | 8081 | Reporting API + Chart.js dashboard |
| `db` | PostgreSQL 16 | 5432 | Shared sales data |

```
Stores -> [Java ingestion] -> PostgreSQL -> [.NET reporting API] -> Dashboard
```

## Run with Docker (recommended)

Requires Docker Desktop.

```bash
docker compose up --build
```

- Dashboard: http://localhost:8081
- Java API health: http://localhost:8080/api/health

A fake sale is generated every 3 seconds, so charts fill in automatically.

## Add a sale manually

```bash
curl -X POST http://localhost:8080/api/sales \
  -H "Content-Type: application/json" \
  -d '{"store":"Delhi","product":"Laptop","quantity":2,"unitPrice":55000}'
```

## Report endpoints (.NET)

- `GET /api/reports/top-products`
- `GET /api/reports/revenue-by-store`
- `GET /api/reports/recent`

## Run without Docker

1. Start PostgreSQL, create DB `retail`, run `db/init.sql`.
2. Java: `cd ingestion-service-java && mvn spring-boot:run`
3. .NET: `cd reporting-api-dotnet && dotnet run --urls http://localhost:8081`

## Ideas to extend

- Replace direct DB writes with Kafka/RabbitMQ between services
- Add JWT auth in the .NET API
- Add demand forecasting (ML.NET or Python)
- Add unit tests (JUnit, xUnit) and a GitHub Actions workflow
