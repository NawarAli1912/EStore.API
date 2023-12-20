using MediatR;
using SharedKernel;

namespace Application.Products.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<Result<Deleted>>;
