using LegacyDataAccess.Core.DTOs;

namespace LegacyDataAccess.Core.Interfaces;

/// <summary>
/// Repository interface for product operations
/// </summary>
public interface IProductRepository
{
    // Read operations
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductListDto>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductListDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductListDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductListDto>> GetLowStockAsync(CancellationToken cancellationToken = default);

    // Write operations
    Task<int> CreateAsync(ProductDto product, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, ProductDto product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    // Stock operations
    Task<bool> UpdateStockAsync(int id, decimal quantity, string operation, CancellationToken cancellationToken = default);
}