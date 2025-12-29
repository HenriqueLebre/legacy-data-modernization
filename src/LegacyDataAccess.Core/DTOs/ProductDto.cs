namespace LegacyDataAccess.Core.DTOs;

/// <summary>
/// Full product data
/// </summary>
public record ProductDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Category { get; init; }
    public string Unit { get; init; } = "UN";
    public decimal CostPrice { get; init; }
    public decimal SalePrice { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal MinStock { get; init; }
    public decimal MaxStock { get; init; }
    public bool IsActive { get; init; } = true;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Lightweight DTO for product lists
/// </summary>
public record ProductListDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal SalePrice { get; init; }
    public decimal StockQuantity { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for stock operations
/// </summary>
public record StockUpdateDto
{
    public int ProductId { get; init; }
    public decimal Quantity { get; init; }
    public string Operation { get; init; } = "ADD"; // ADD, REMOVE, SET
}