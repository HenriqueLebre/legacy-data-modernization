using Microsoft.AspNetCore.Mvc;
using LegacyDataAccess.Core.DTOs;
using LegacyDataAccess.Core.Interfaces;

namespace LegacyDataAccess.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISalesRepository _salesRepository;

    public ReportsController(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    [HttpGet("sales/monthly/{year:int}/{month:int}")]
    public async Task<ActionResult<MonthlySalesReportDto>> GetMonthlySalesReport(
        int year, int month, CancellationToken cancellationToken = default)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { message = "Month must be between 1 and 12" });

        var report = await _salesRepository.GetMonthlySalesReportAsync(year, month, cancellationToken);
        return Ok(report);
    }

    [HttpGet("sales/daily")]
    public async Task<ActionResult<IEnumerable<DailySalesDto>>> GetDailySales(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        if (endDate <= startDate)
            return BadRequest(new { message = "End date must be after start date" });

        var sales = await _salesRepository.GetDailySalesAsync(startDate, endDate, cancellationToken);
        return Ok(sales);
    }

    [HttpGet("products/top-selling")]
    public async Task<ActionResult<IEnumerable<TopSellingProductDto>>> GetTopSellingProducts(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int top = 10,
        CancellationToken cancellationToken = default)
    {
        var products = await _salesRepository.GetTopSellingProductsAsync(startDate, endDate, top, cancellationToken);
        return Ok(products);
    }

    [HttpGet("customers/top")]
    public async Task<ActionResult<IEnumerable<TopCustomerDto>>> GetTopCustomers(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int top = 10,
        CancellationToken cancellationToken = default)
    {
        var customers = await _salesRepository.GetTopCustomersAsync(startDate, endDate, top, cancellationToken);
        return Ok(customers);
    }

    [HttpGet("sales/summary")]
    public async Task<IActionResult> GetSalesSummary(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var total = await _salesRepository.GetTotalSalesAsync(startDate, endDate, cancellationToken);
        var count = await _salesRepository.GetSalesCountAsync(startDate, endDate, cancellationToken);
        var average = await _salesRepository.GetAverageTicketAsync(startDate, endDate, cancellationToken);

        return Ok(new { startDate, endDate, totalSales = total, transactionCount = count, averageTicket = average });
    }
}