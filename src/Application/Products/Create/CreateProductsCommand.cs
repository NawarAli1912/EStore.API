using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.Create;


public record CreateProductsCommand(
    List<CreateProductItems> Items) : IRequest<Result<CreateProductsResult>>;

public record CreateProductItems(
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    decimal PurchasePrice,
    List<Guid> Categories);
