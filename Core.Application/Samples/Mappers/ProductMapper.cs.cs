using Riok.Mapperly.Abstractions;
using Core.Application.Samples.DTOs;
using Core.Domain.Samples.Entities;

namespace Core.Application.Samples.Mappers;

[Mapper]
public partial class ProductMapper
{
    // ----------------------------
    // Domain -> DTO (Read)
    // ----------------------------
    public partial ProductDto ToDto(Product product);

    public partial List<ProductDto> ToDtoList(IEnumerable<Product> products);

    // ----------------------------
    // DTO -> Domain (Create)
    // ----------------------------
    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapProperty(nameof(CreateProductDto.Name), nameof(Product.Name))]
    [MapProperty(nameof(CreateProductDto.Description), nameof(Product.Description))]
    [MapProperty(nameof(CreateProductDto.Category), nameof(Product.Category))]
    [MapProperty(nameof(CreateProductDto.UnitPrice), nameof(Product.UnitPrice))]
    [MapProperty(nameof(CreateProductDto.StockQuantity), nameof(Product.StockQuantity))]
    [MapProperty(nameof(CreateProductDto.UnitOfMeasure), nameof(Product.UnitOfMeasure))]
    [MapProperty(nameof(CreateProductDto.Manufacturer), nameof(Product.Manufacturer))]
    public partial Product ToEntity(CreateProductDto dto);

    // ----------------------------
    // DTO -> Domain (Update)
    // ----------------------------

    [MapProperty(nameof(UpdateProductDto.Id), nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.CreatedAt))]
    public partial void UpdateEntity(UpdateProductDto dto, Product product);
}