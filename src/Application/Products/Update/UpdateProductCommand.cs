using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.Update;

public record UpdateProductCommand(
    Guid Id,
    string? Name,
    string? Description,
    int? Quantity,
    decimal? PurchasePrice,
    decimal? CustomerPrice) : IRequest<Result<Updated>>;

