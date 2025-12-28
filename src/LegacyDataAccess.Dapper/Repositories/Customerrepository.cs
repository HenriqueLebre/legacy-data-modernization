using Dapper;
using LegacyDataAccess.Core.DTOs;
using LegacyDataAccess.Core.Interfaces;
using LegacyDataAccess.Dapper.Configuration;
using LegacyDataAccess.Dapper.Queries;

namespace LegacyDataAccess.Dapper.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DapperConfiguration _config;

    public CustomerRepository(DapperConfiguration config)
    {
        _config = config;
    }

    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<CustomerDto>(
            CustomerQueries.GetById,
            new { Id = id });
    }

    public async Task<CustomerDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<CustomerDto>(
            CustomerQueries.GetByCode,
            new { Code = code });
    }

    public async Task<IEnumerable<CustomerListDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<CustomerListDto>(
            CustomerQueries.GetAll,
            new { IncludeInactive = includeInactive });
    }

    public async Task<IEnumerable<CustomerListDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<CustomerListDto>(
            CustomerQueries.Search,
            new { SearchTerm = $"%{searchTerm}%" });
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(
            CustomerQueries.Exists,
            new { Id = id });
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(
            CustomerQueries.CodeExists,
            new { Code = code });
    }

    public async Task<int> CreateAsync(CustomerCreateDto customer, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(
            CustomerQueries.Insert,
            customer);
    }

    public async Task<bool> UpdateAsync(int id, CustomerCreateDto customer, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        var affected = await connection.ExecuteAsync(
            CustomerQueries.Update,
            new
            {
                Id = id,
                customer.Code,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Document,
                customer.Address,
                customer.City,
                customer.State,
                customer.ZipCode,
                customer.CreditLimit
            });
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        var affected = await connection.ExecuteAsync(
            CustomerQueries.Delete,
            new { Id = id });
        return affected > 0;
    }

    public async Task<decimal> GetBalanceAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<decimal>(
            CustomerQueries.GetBalance,
            new { Id = id });
    }

    public async Task<bool> UpdateBalanceAsync(int id, decimal amount, CancellationToken cancellationToken = default)
    {
        await using var connection = await _config.CreateConnectionAsync(cancellationToken);
        var affected = await connection.ExecuteAsync(
            CustomerQueries.UpdateBalance,
            new { Id = id, Amount = amount });
        return affected > 0;
    }
}