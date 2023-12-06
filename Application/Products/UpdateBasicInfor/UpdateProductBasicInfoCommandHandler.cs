using Application.Common.Data;
using Domain.DomainErrors.Products;
using Domain.Kernal;
using Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.UpdateBasicInfor;

internal class UpdateProductBasicInfoCommandHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateProductBasicInfoCommand, Result<Product>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Product>> Handle(UpdateProductBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var product = await _context
            .Products
            .FirstOrDefaultAsync(p => p.Id == request.Id);

        if (product is null)
        {
            return Errors.Product.NotFound;
        }

        product.UpdateBasicInfo(request.Name, request.Description);

        await _context.SaveChangesAsync(cancellationToken);

        return product;
    }
}
