using Application.Common.Data;
using Domain.DomainErrors.Products;
using Domain.DomainEvents;
using Domain.Kernal;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Delete;
internal class DeleteProductCommandHandler(IApplicationDbContext context)
        : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var result = await _context
            .Products
            .FirstOrDefaultAsync(p => p.Id == request.Id);

        if (result is null)
            return Errors.Product.NotFound;

        result.RaiseDomainEvent(new ProductDeletedDomainEvent(result.Id));
        _context.Products.Remove(result);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
