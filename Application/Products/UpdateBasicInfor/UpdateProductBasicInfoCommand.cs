using Domain.Kernal;
using Domain.Products;
using MediatR;

namespace Application.Products.UpdateBasicInfor;

public record UpdateProductBasicInfoCommand(
    Guid Id,
    string? Name,
    string? Description) : IRequest<Result<Product>>;

