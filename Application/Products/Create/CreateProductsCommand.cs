using Domain.Kernal;
using MediatR;

namespace Application.Products.Create;


public record CreateProductsCommand(
    List<CreateProductItems> Items) : IRequest<Result<CreateProductsResult>>;

public record CreateProductItems(
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    decimal PurchasePrice,
    string Currency,
    string? Sku,
    List<Guid> Categories);
