using Domain.Kernal;
using MediatR;

namespace Application.Products.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<Result<Deleted>>;
