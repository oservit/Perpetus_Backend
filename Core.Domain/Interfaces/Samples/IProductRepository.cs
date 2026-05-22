using Core.Domain.Entities.Samples;
using Core.Domain.Interfaces.Common;

namespace Core.Domain.Interfaces.Samples;

public interface IProductRepository
    : IBaseRepository<Product>
{
}