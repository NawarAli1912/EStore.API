using Application.Common.Idempotency;
using SharedKernel.Primitives;

namespace Application.Products.Create;

public record CreateProductsCommand(
    Guid RequestId,
    List<CreateProductItems> Items)
    : IdempotentCommand<Result<CreateProductsResult>>(RequestId);

public record CreateProductItems(
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    decimal PurchasePrice,
    List<Guid> Categories);
