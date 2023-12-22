using Application.Common.Data;
using Domain.Categories;
using Domain.Products;
using Domain.Products.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Products.Create;

public sealed class CreateProductsCommandHandler(IApplicationDbContext context) :
    IRequestHandler<CreateProductsCommand, Result<CreateProductsResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<CreateProductsResult>> Handle(
        CreateProductsCommand request,
        CancellationToken cancellationToken)
    {
        List<Error> errors = [];
        List<Product> products = [];
        HashSet<Guid> categoriesIds = request
            .Items
            .SelectMany(item => item.Categories)
            .ToHashSet();

        var categoriesDict = await _context
            .Categories
            .Where(c => categoriesIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c, cancellationToken);

        foreach (var item in request.Items)
        {
            Sku? sku = null;
            var skuResult = Sku.Create(item.Sku);
            if (skuResult.IsError)
            {
                errors.AddRange(skuResult.Errors);
            }
            else
            {
                sku = skuResult.Value;
            }

            var productResult = Product.Create(
                Guid.NewGuid(),
                item.Name,
                item.Description,
                item.Quantity,
                item.CustomerPrice,
                item.PurchasePrice,
                sku);

            if (productResult.IsError)
            {
                errors.AddRange(productResult.Errors);
                continue;
            }

            var currentProduct = productResult.Value;
            List<Category> productCategories = [];
            foreach (var categoryId in item.Categories)
            {
                if (categoriesDict.TryGetValue(categoryId, out var category))
                {
                    productCategories.Add(category);
                }
            }

            currentProduct.AssignCategories(productCategories);
            products.Add(currentProduct);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        foreach (var product in products)
        {
            await _context.Products.AddAsync(
                product,
                cancellationToken);
        }


        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProductsResult(products);
    }
}
