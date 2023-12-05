using Domain.Products;

namespace Application.Products.Create;

public record CreateProductsResult(
    List<Product> Items
    );
