using Application.Common.DatabaseAbstraction;
using Application.Common.FriendlyIdentifiers;
using Domain.Categories;
using Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Products.Create;

public sealed class CreateProductsCommandHandler(IApplicationDbContext context, IFriendlyIdGenerator friendlyIdGenerator) :
    IRequestHandler<CreateProductsCommand, Result<CreateProductsResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IFriendlyIdGenerator _friendlyIdGenerator = friendlyIdGenerator;

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
            var currentProduct = Product.Create(
                Guid.NewGuid(),
                item.Name,
                item.Description,
                item.Quantity,
                item.CustomerPrice,
                item.PurchasePrice);

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

        var friendlyIds = await
            _friendlyIdGenerator.GenerateProductFriendlyId(products.Count);
        foreach (var product in products)
        {
            product.SetCode(friendlyIds.Last());
            await _context.Products.AddAsync(
                product,
                cancellationToken);
            friendlyIds.RemoveAt(friendlyIds.Count - 1);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProductsResult(products);
    }
}
