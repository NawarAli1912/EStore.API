using Domain.Products;

namespace Application.Products.List;

public record ListProductResult(List<Product> Products, int TotalCount);
