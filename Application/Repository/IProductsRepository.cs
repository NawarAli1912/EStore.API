using Domain.Products;

namespace Application.Repository;

public interface IProductsRepository
{
    Task<List<Product>> GetByCategories(IEnumerable<Guid> categoryIds);

    Task<int> GetProductCountByCategory(IEnumerable<Guid> categoriesIds);
}
