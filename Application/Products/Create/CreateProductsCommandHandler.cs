using Application.Common.Data;
using Domain.Kernal;
using Domain.Products;
using MediatR;

namespace Application.Products.Create;

public sealed class CreateProductsCommandHandler(IApplicationDbContext context) :
    IRequestHandler<CreateProductsCommand, Result<CreateProductsResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<CreateProductsResult>> Handle(CreateProductsCommand request, CancellationToken cancellationToken)
    {
        List<Error> errors = [];
        List<Product> products = [];
        foreach (var item in request.Items)
        {
            var productResult = Product.Create(
                Guid.NewGuid(),
                item.Name,
                item.Description,
                item.Quantity,
                item.CustomerPrice,
                item.PurchasePrice,
                item.Currency,
                item.Sku);

            if (productResult.IsError)
            {
                errors.AddRange(productResult.Errors);
                continue;
            }
            products.Add(productResult.Value);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        foreach (var product in products)
        {
            await _context.Products.AddAsync(product);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProductsResult(products);
    }
}
