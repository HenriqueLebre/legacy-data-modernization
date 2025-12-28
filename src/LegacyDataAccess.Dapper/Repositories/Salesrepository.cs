using Dapper;
using LegacyDataAccess.Core.DTOs;
using LegacyDataAccess.Core.Interfaces;
using LegacyDataAccess.Dapper.Configuration;
using LegacyDataAccess.Dapper.Queries;

namespace LegacyDataAccess.Dapper.Repositories;

public class SalesRepository : ISalesRepository
{
    private readonly DapperConfiguration _config;

    public SalesRepository(DapperConfiguration config)
    {
        _config = config;
    }

    public async Task<MonthlySalesReportDto> GetMonthlySalesReportAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);
        var previousStart = startDate.AddMonths(-1);
        var previousEnd = startDate;

        await using var connection = await _config.CreateConnectionAsync(cancellationToken);

        // Get all data in parallel
        var totalTask = GetTotalSalesAsync(startDate, endDate, cancellationToken);
        var countTask = GetSalesCountAsync(startDate, endDate, cancellationToken);
        var averageTask = GetAverageTicketAsync(startDate, endDate, cancellationToken);
        var dailyTask = GetDailySalesAsync(startDate, endDate, cancellationToken);
        var topProductsTask = GetTopSellingProductsAsync(startDate, endDate, 5, cancellationToken);
        var topCustomersTask = GetTopCustomersAsync(startDate, endDate, 5, cancellationToken);

        await Task.WhenAll(totalTask, countTask, averageTask, dailyTask, topProductsTask, topCustomersTask);

        // Get previous month for comparison
        var previousTotal = await connection.ExecuteScalarAsync<decimal>(
            ReportQueries.PreviousMonthTotal,
            new { PreviousStart = previousStart, PreviousEnd = previousEnd });

        var currentTotal = await totalTask;
        var comparedToPrevious = previousTotal > 0
            ? ((currentTotal - previousTotal) / previousTotal) * 100
            : 0;

        return new MonthlySalesReportDto
        {
            Year = year,
            Month = month,
            TotalSales = currentTotal,
            TotalTransactions = await countTask,
            AverageTicket = await averageTask,
            ComparedToPreviousMonth = comparedToPrevious,
            DailyBreakdown = await dailyTask,
            TopProducts = await topProductsTask,
            TopCustomers = await topCustomersTask
        };
    }

    public async Task<IEnumerable<DailySalesDto>> GetDailySalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<DailySalesDto>(
            ReportQueries.DailySales,
            new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<IEnumerable<TopSellingProductDto>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int top = 10, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<TopSellingProductDto>(
            ReportQueries.TopSellingProducts,
            new { StartDate = startDate, EndDate = endDate, Top = top });
    }

    public async Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(DateTime startDate, DateTime endDate, int top = 10, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<TopCustomerDto>(
            ReportQueries.TopCustomers,
            new { StartDate = startDate, EndDate = endDate, Top = top });
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<decimal>(
            ReportQueries.TotalSales,
            new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<int> GetSalesCountAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(
            ReportQueries.SalesCount,
            new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<decimal> GetAverageTicketAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<decimal>(
            ReportQueries.AverageTicket,
            new { StartDate = startDate, EndDate = endDate });
    }
}