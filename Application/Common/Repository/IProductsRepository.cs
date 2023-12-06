using Application.Products.List;
using Domain.Products;

namespace Application.Common.Repository;

public interface IProductsRepository
{
    Task<(List<Product>, int)> ListByFilter(ProductsFilter filter, int pageIndex, int pageSize);

    Task<List<Product>> ListByCategories(IEnumerable<Guid> categoryIds);

    Task<int> GetProductCountByCategory(IEnumerable<Guid> categoriesIds);
}
