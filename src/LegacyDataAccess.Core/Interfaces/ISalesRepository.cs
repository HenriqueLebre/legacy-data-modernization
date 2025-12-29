using LegacyDataAccess.Core.DTOs;

namespace LegacyDataAccess.Core.Interfaces;

/// <summary>
/// Repository interface for sales and reports
/// </summary>
public interface ISalesRepository
{
    // Report operations
    Task<MonthlySalesReportDto> GetMonthlySalesReportAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<IEnumerable<DailySalesDto>> GetDailySalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int top = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(DateTime startDate, DateTime endDate, int top = 10, CancellationToken cancellationToken = default);
    
    // Aggregates
    Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<int> GetSalesCountAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetAverageTicketAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}