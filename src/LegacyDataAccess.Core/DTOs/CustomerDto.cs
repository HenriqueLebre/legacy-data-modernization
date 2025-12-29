namespace LegacyDataAccess.Core.DTOs;

/// <summary>
/// Full customer data for single record operations
/// </summary>
public record CustomerDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Document { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
    public bool IsActive { get; init; } = true;
    public decimal CreditLimit { get; init; }
    public decimal CurrentBalance { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Lightweight DTO for customer lists
/// </summary>
public record CustomerListDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for creating/updating customers
/// </summary>
public record CustomerCreateDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Document { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
    public decimal CreditLimit { get; init; }
}