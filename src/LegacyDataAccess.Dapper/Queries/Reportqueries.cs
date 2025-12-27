namespace LegacyDataAccess.Dapper.Queries;

/// <summary>
/// Complex SQL queries for reports
/// Demonstrates advanced SQL: CTEs, Window Functions, Aggregations
/// </summary>
public static class ReportQueries
{
    /// <summary>
    /// Daily sales with running total using Window Function
    /// </summary>
    public const string DailySales = @"
        SELECT 
            DATE(sale_date) as date,
            COUNT(*) as transaction_count,
            SUM(total) as total_amount,
            AVG(total) as average_ticket,
            SUM(SUM(total)) OVER (ORDER BY DATE(sale_date)) as running_total
        FROM sales
        WHERE sale_date >= @StartDate 
          AND sale_date < @EndDate
          AND status = 'COMPLETED'
        GROUP BY DATE(sale_date)
        ORDER BY date";

    /// <summary>
    /// Top selling products using CTE
    /// </summary>
    public const string TopSellingProducts = @"
        WITH product_sales AS (
            SELECT 
                p.id as product_id,
                p.code as product_code,
                p.description as product_description,
                SUM(si.quantity) as quantity_sold,
                SUM(si.total) as total_revenue
            FROM products p
            INNER JOIN sale_items si ON si.product_id = p.id
            INNER JOIN sales s ON s.id = si.sale_id
            WHERE s.sale_date >= @StartDate 
              AND s.sale_date < @EndDate
              AND s.status = 'COMPLETED'
            GROUP BY p.id, p.code, p.description
        )
        SELECT 
            product_id,
            product_code,
            product_description,
            quantity_sold,
            total_revenue,
            ROW_NUMBER() OVER (ORDER BY total_revenue DESC) as rank
        FROM product_sales
        ORDER BY total_revenue DESC
        LIMIT @Top";

    /// <summary>
    /// Top customers by total spent
    /// </summary>
    public const string TopCustomers = @"
        WITH customer_purchases AS (
            SELECT 
                c.id as customer_id,
                c.name as customer_name,
                COUNT(s.id) as purchase_count,
                SUM(s.total) as total_spent
            FROM customers c
            INNER JOIN sales s ON s.customer_id = c.id
            WHERE s.sale_date >= @StartDate 
              AND s.sale_date < @EndDate
              AND s.status = 'COMPLETED'
            GROUP BY c.id, c.name
        )
        SELECT 
            customer_id,
            customer_name,
            purchase_count,
            total_spent,
            ROW_NUMBER() OVER (ORDER BY total_spent DESC) as rank
        FROM customer_purchases
        ORDER BY total_spent DESC
        LIMIT @Top";

    public const string TotalSales = @"
        SELECT COALESCE(SUM(total), 0)
        FROM sales
        WHERE sale_date >= @StartDate 
          AND sale_date < @EndDate
          AND status = 'COMPLETED'";

    public const string SalesCount = @"
        SELECT COUNT(*)
        FROM sales
        WHERE sale_date >= @StartDate 
          AND sale_date < @EndDate
          AND status = 'COMPLETED'";

    public const string AverageTicket = @"
        SELECT COALESCE(AVG(total), 0)
        FROM sales
        WHERE sale_date >= @StartDate 
          AND sale_date < @EndDate
          AND status = 'COMPLETED'";

    /// <summary>
    /// Compare with previous month
    /// </summary>
    public const string PreviousMonthTotal = @"
        SELECT COALESCE(SUM(total), 0)
        FROM sales
        WHERE sale_date >= @PreviousStart 
          AND sale_date < @PreviousEnd
          AND status = 'COMPLETED'";
}