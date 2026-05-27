using Core.Domain.Common.Interfaces;
using Core.Domain.Samples.Entities;

namespace Core.Domain.Samples.Interfaces;

public interface IProductRepository
    : IBaseRepository<Product>
{
}