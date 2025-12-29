# Architecture Overview

## Design Patterns

### Anti-Corruption Layer (ACL)

The core pattern isolates legacy code from modern implementation:

```
Legacy Code  →  ACL (Translation)  →  Modern Code
XHarbour        DatabaseWrapper       .NET 8 API
DBF mindset     Field mapping         Async/JSON
```

### Repository Pattern

Each entity has its own repository with clear interface:

```csharp
public interface ICustomerRepository
{
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerListDto>> SearchAsync(string term);
    Task<int> CreateAsync(CustomerCreateDto customer);
}
```

### DTO Pattern

Separate models for different use cases:

- `CustomerDto` - Full data (single record)
- `CustomerListDto` - Lightweight for lists
- `CustomerCreateDto` - Input validation

## Data Flow

```
1. XHarbour: oWA:Seek("CLI-001")
2. Wrapper:  GET /api/customers/code/CLI-001
3. API:      CustomerRepository.GetByCodeAsync()
4. Dapper:   SELECT * FROM customers WHERE code = @Code
5. Response: JSON → Hash → XHarbour field access
```

## SQL Organization

Queries are organized in separate classes for DBA collaboration:

```csharp
public static class CustomerQueries
{
    public const string GetById = @"SELECT ... FROM customers WHERE id = @Id";
    public const string Search = @"SELECT ... WHERE name ILIKE @Term";
}
```

Complex reports use CTEs and Window Functions:

```sql
WITH daily_sales AS (...)
SELECT date, total,
       SUM(total) OVER (ORDER BY date) as running_total
FROM daily_sales
```