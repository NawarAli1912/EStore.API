using Application.Common.Data;
using Domain.DomainErrors;
using Domain.Kernal;
using Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Update;

internal class UpdateProductCommandHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateProductCommand, Result<Product>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        List<Error> errors = [];
        var product = await _context
            .Products
            .FirstOrDefaultAsync(p => p.Id == request.Id);

        if (product is null)
        {
            return Errors.Product.NotFound;
        }

        var updateProductResult = product.Update(
            request.Name,
            request.Description,
            request.Quantity,
            request.CustomerPrice,
            request.PurchasePrice,
            request.Sku,
            request.NullSku);

        if (updateProductResult.IsError)
        {
            return updateProductResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return updateProductResult.Value;
    }
}
