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