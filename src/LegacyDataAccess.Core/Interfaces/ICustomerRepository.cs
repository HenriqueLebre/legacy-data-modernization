using LegacyDataAccess.Core.DTOs;

namespace LegacyDataAccess.Core.Interfaces;

/// <summary>
/// Repository interface for customer operations
/// </summary>
public interface ICustomerRepository
{
    // Read operations
    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerListDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerListDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

    // Write operations
    Task<int> CreateAsync(CustomerCreateDto customer, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, CustomerCreateDto customer, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    // Balance operations
    Task<decimal> GetBalanceAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateBalanceAsync(int id, decimal amount, CancellationToken cancellationToken = default);
}