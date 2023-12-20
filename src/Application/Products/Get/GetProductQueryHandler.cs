using Application.Common.Data;
using Domain.Products;
using Domain.Products.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.Get;

internal sealed class GetProductQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductQuery, Result<Product>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Product>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {

        var product = await _context
            .Products
            .Include(p => p.Categories)
            .ThenInclude(c => c.SubCategories)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            return DomainError.Product.NotFound;
        }
        return product;
    }
}
