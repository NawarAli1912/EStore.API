using MediatR;
using SharedKernel;

namespace Application.Products.Create;


public record CreateProductsCommand(
    List<CreateProductItems> Items) : IRequest<Result<CreateProductsResult>>;

public record CreateProductItems(
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    decimal PurchasePrice,
    string? Sku,
    List<Guid> Categories);
