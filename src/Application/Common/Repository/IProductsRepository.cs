using Application.Products.List;
using Domain.Products;

namespace Application.Common.Repository;

public interface IProductsRepository
{
    Task<(List<Product>, int)> ListByFilter(ProductsFilter filter, int pageIndex, int pageSize);

    Task<(List<Product>, int)> ListByCategories(IEnumerable<Guid> categoryIds, int pageIndex, int pageSize);

    Task<int> GetProductCountByCategory(IEnumerable<Guid> categoriesIds);
}
