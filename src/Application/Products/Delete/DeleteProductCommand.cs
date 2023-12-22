using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<Result<Deleted>>;
