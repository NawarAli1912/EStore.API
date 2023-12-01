using Application.Common.Data;
using Domain.DomainErrors.Products;
using Domain.Kernal;
using Domain.Products;
using MediatR;

namespace Application.Products.Get;

internal sealed class GetProductQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductQuery, Result<Product>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Product>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(request.ProductId);

        if (product is null)
        {
            return Errors.Product.NotFound;
        }
        return product;
    }
}
