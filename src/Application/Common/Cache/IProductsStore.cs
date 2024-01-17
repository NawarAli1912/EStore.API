using Domain.Products;

namespace Application.Common.Cache;
public interface IProductsStore
{
    Task<List<Product>> GetByIds(ISet<Guid> requestedIds,
        CancellationToken cancellationToken = default);
}
