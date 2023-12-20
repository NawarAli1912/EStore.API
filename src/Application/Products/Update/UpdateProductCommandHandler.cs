using Application.Common.Data;
using Domain.Products;
using Domain.Products.Errors;
using Domain.Products.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

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
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return DomainError.Product.NotFound;
        }

        var skuResult =
            Sku.Create(request.Sku);

        if (skuResult.IsError)
        {
            return skuResult.Errors;
        }

        var updateProductResult = product.Update(
            request.Name,
            request.Description,
            request.Quantity,
            request.CustomerPrice,
            request.PurchasePrice,
            skuResult.Value,
            request.NullSku);

        if (updateProductResult.IsError)
        {
            return updateProductResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return updateProductResult.Value;
    }
}
