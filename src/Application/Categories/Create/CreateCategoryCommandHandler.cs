using Application.Common.Cache;
using Application.Common.DatabaseAbstraction;
using Domain.Categories;
using Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;


namespace Application.Categories.Create;

internal sealed class CreateCategoryCommandHandler
        : IRequestHandler<CreateCategoryCommand, Result<Created>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProductsStore _productsStore;

    public CreateCategoryCommandHandler(IApplicationDbContext context, IProductsStore productsStore)
    {
        _context = context;
        _productsStore = productsStore;
    }


    public async Task<Result<Created>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {

        Category? parentCategory = default!;
        var products = await _productsStore
            .GetByIds(request.Products.ToHashSet(), cancellationToken);

        if (request.ParentCategoryId is not null)
        {
            parentCategory = await _context
                    .Categories
                    .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId, cancellationToken: cancellationToken);

            if (parentCategory is null)
            {
                return DomainError.Categories.NotFound;
            }
        }

        var categroy = Category.Create(
            Guid.NewGuid(),
            request.Name,
            parentCategory: parentCategory);

        categroy.AssignProducts(products);

        await _context.Categories.AddAsync(categroy, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Created;
    }
}
