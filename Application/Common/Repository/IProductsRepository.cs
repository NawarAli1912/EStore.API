using Application.Products.Filters;
using Domain.Products;

namespace Application.Common.Repository;

public interface IProductsRepository
{
    Task<(List<Product>, int)> ListByFilter(ListProductFilter filter, int pageIndex, int pageSize);

    Task<List<Product>> ListByCategories(IEnumerable<Guid> categoryIds);

    Task<int> GetProductCountByCategory(IEnumerable<Guid> categoriesIds);
}
