using Core.Application.Samples.DTOs;
using Core.Domain.Entities.Samples;
using Core.Domain.Interfaces.Samples;

namespace Core.Application.Samples.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // =========================================================
    // CREATE
    // =========================================================
    public async Task<long> InsertAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            UnitPrice = dto.UnitPrice,
            StockQuantity = dto.StockQuantity,
            UnitOfMeasure = dto.UnitOfMeasure,
            Manufacturer = dto.Manufacturer,
            CreatedAt = DateTime.UtcNow
        };

        return await _productRepository.InsertAsync(product);
    }

    // =========================================================
    // READ (BY ID)
    // =========================================================
    public async Task<ProductDto?> GetByIdAsync(object id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            UnitPrice = product.UnitPrice,
            StockQuantity = product.StockQuantity,
            UnitOfMeasure = product.UnitOfMeasure,
            Manufacturer = product.Manufacturer,
            CreatedAt = product.CreatedAt
        };
    }

    // =========================================================
    // READ (ALL)
    // =========================================================
    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            UnitPrice = product.UnitPrice,
            StockQuantity = product.StockQuantity,
            UnitOfMeasure = product.UnitOfMeasure,
            Manufacturer = product.Manufacturer,
            CreatedAt = product.CreatedAt
        });
    }

    // =========================================================
    // UPDATE
    // =========================================================
    public async Task<int> UpdateAsync(ProductDto dto)
    {
        var product = new Product
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            UnitPrice = dto.UnitPrice,
            StockQuantity = dto.StockQuantity,
            UnitOfMeasure = dto.UnitOfMeasure,
            Manufacturer = dto.Manufacturer,
            CreatedAt = dto.CreatedAt
        };

        return await _productRepository.UpdateAsync(product);
    }

    // =========================================================
    // DELETE
    // =========================================================
    public async Task<int> DeleteAsync(object id)
    {
        return await _productRepository.DeleteAsync(id);
    }
}