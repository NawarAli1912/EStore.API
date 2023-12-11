﻿using Application.Common.Data;
using Domain.DomainErrors;
using Domain.Kernal;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts.Clear;

internal sealed class ClearCartCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ClearCartCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context
            .Customers
            .Include(c => c.Cart)
            .ThenInclude(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Errors.Customers.NotFound;
        }

        customer.ClearCart();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}