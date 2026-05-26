using Core.Application.Samples.DTOs;
using Core.Application.Samples.Mappers;
using Core.Domain.Interfaces.Samples;

namespace Core.Application.Samples.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ProductMapper _mapper;

    public ProductService(IProductRepository productRepository, ProductMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    // =========================================================
    // CREATE
    // =========================================================
    public async Task<long> InsertAsync(CreateProductDto dto)
    {
        var entity = _mapper.ToEntity(dto);

        return await _productRepository.InsertAsync(entity);
    }

    // =========================================================
    // READ (BY ID)
    // =========================================================
    public async Task<ProductDto?> GetByIdAsync(object id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return null;

        if (product is null)
            return null;

        return _mapper.ToDto(product);
    }

    // =========================================================
    // READ (ALL)
    // =========================================================
    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return _mapper.ToDtoList(products.ToList());
    }

    // =========================================================
    // UPDATE
    // =========================================================
    public async Task<int> UpdateAsync(UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id);

        if (product is null)
            throw new Exception("Product not found");

        _mapper.UpdateEntity(dto, product);

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