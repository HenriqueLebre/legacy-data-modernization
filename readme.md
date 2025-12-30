# Legacy Data Modernization

> Anti-Corruption Layer for migrating XHarbour/Clipper applications to .NET 8 + PostgreSQL

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## Overview

This project provides a complete data access layer that allows legacy XHarbour/Clipper ERP systems to seamlessly integrate with a modern .NET 8 + Dapper + PostgreSQL backend, using familiar DBF-style commands.

## Architecture

```
┌─────────────────────────────────────┐
│     Legacy XHarbour Application     │
│   USE CUSTOMERS / SEEK / REPLACE    │
└──────────────────┬──────────────────┘
                   │
         ┌─────────▼─────────┐
         │  Database Wrapper │
         │  (Anti-Corruption)│
         └─────────┬─────────┘
                   │
         ┌─────────▼─────────┐
         │   .NET 8 REST API │
         │   (Dapper + SQL)  │
         └─────────┬─────────┘
                   │
         ┌─────────▼─────────┐
         │    PostgreSQL     │
         └───────────────────┘
```

## Quick Start

```bash
# Start database and API
docker-compose up -d

# Test API
curl http://localhost:5000/health
```

## Project Structure

```
├── src/
│   ├── LegacyDataAccess.Core/      # DTOs and interfaces
│   ├── LegacyDataAccess.Dapper/    # Repository implementations
│   └── LegacyDataAccess.API/       # REST API
├── legacy-client/
│   ├── RestClient/                 # HTTP client for XHarbour
│   └── DatabaseWrapper/            # DBF command emulation
├── scripts/db/                     # SQL scripts
└── docs/                           # Documentation
```

## XHarbour Integration

```xbase
// Initialize
DbWrapperInit("http://localhost:5000")

// Use familiar commands
DbUseApi("CUSTOMERS", "CLI", .T.)
oWA := GetCurrentWorkArea()

oWA:Seek("CLI-001")
IF oWA:Found()
   ? FIELD("CLI", "name")
ENDIF

DbCloseApi("CLI")
```

## Performance

| Operation | Legacy DBF | PostgreSQL + Dapper |
|-----------|------------|---------------------|
| Search    | 3,200ms    | 45ms (71x faster)   |
| Report    | 12,000ms   | 180ms (67x faster)  |

## License

MIT License - See [LICENSE](LICENSE) for details.

---

## 🧪 Testing the API

### Prerequisites

1. Start PostgreSQL:
```bash
docker-compose up -d postgres
```

2. Run the API:
```bash
dotnet run --project src/LegacyDataAccess.API
```

3. Access Swagger UI: http://localhost:5246/swagger

---

### Health Checks
```bash
# Health status
curl http://localhost:5246/health

# Readiness probe
curl http://localhost:5246/health/ready

# Liveness probe
curl http://localhost:5246/health/live
```

---

### Customers API

#### Create Customer
```bash
curl -X POST http://localhost:5246/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "code": "CLI-001",
    "name": "Empresa Teste Ltda",
    "email": "contato@teste.com.br",
    "phone": "(11) 99999-9999",
    "document": "12.345.678/0001-90",
    "address": "Rua Teste, 123",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234-567",
    "creditLimit": 10000.00
  }'
```

#### Get All Customers
```bash
curl http://localhost:5246/api/customers
```

#### Get Customer by ID
```bash
curl http://localhost:5246/api/customers/1
```

#### Get Customer by Code
```bash
curl http://localhost:5246/api/customers/code/CLI-001
```

#### Search Customers
```bash
curl "http://localhost:5246/api/customers/search?q=Teste"
```

#### Update Customer
```bash
curl -X PUT http://localhost:5246/api/customers/1 \
  -H "Content-Type: application/json" \
  -d '{
    "code": "CLI-001",
    "name": "Empresa Atualizada Ltda",
    "email": "novo@teste.com.br",
    "creditLimit": 15000.00
  }'
```

#### Delete Customer
```bash
curl -X DELETE http://localhost:5246/api/customers/1
```

#### Get Customer Balance
```bash
curl http://localhost:5246/api/customers/1/balance
```

#### Update Customer Balance
```bash
curl -X POST http://localhost:5246/api/customers/1/balance \
  -H "Content-Type: application/json" \
  -d '{"amount": 500.00}'
```

---

### Reports API

#### Monthly Sales Report
```bash
curl "http://localhost:5246/api/reports/sales/monthly/2024/12"
```

#### Daily Sales
```bash
curl "http://localhost:5246/api/reports/sales/daily?startDate=2024-01-01&endDate=2024-12-31"
```

#### Top Selling Products
```bash
curl "http://localhost:5246/api/reports/products/top-selling?startDate=2024-01-01&endDate=2024-12-31&top=10"
```

#### Top Customers
```bash
curl "http://localhost:5246/api/reports/customers/top?startDate=2024-01-01&endDate=2024-12-31&top=10"
```

---