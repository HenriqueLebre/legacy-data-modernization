namespace LegacyDataAccess.Core.DTOs;

/// <summary>
/// Monthly sales report
/// </summary>
public record MonthlySalesReportDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public decimal TotalSales { get; init; }
    public int TotalTransactions { get; init; }
    public decimal AverageTicket { get; init; }
    public decimal ComparedToPreviousMonth { get; init; }
    public IEnumerable<DailySalesDto> DailyBreakdown { get; init; } = [];
    public IEnumerable<TopSellingProductDto> TopProducts { get; init; } = [];
    public IEnumerable<TopCustomerDto> TopCustomers { get; init; } = [];
}

/// <summary>
/// Daily sales summary
/// </summary>
public record DailySalesDto
{
    public DateTime Date { get; init; }
    public int TransactionCount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal AverageTicket { get; init; }
    public decimal RunningTotal { get; init; }
}

/// <summary>
/// Top selling product
/// </summary>
public record TopSellingProductDto
{
    public int ProductId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductDescription { get; init; } = string.Empty;
    public decimal QuantitySold { get; init; }
    public decimal TotalRevenue { get; init; }
    public int Rank { get; init; }
}

/// <summary>
/// Top customer by purchases
/// </summary>
public record TopCustomerDto
{
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int PurchaseCount { get; init; }
    public decimal TotalSpent { get; init; }
    public int Rank { get; init; }
}