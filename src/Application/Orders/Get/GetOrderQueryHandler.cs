﻿using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.Get;

internal sealed class GetOrderQueryHandler :
    IRequestHandler<GetOrderQuery, Result<Order>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _context
            .Orders
            .Include(o => o.LineItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return DomainError.Orders.NotFound;
        }

        return order;
    }
}
