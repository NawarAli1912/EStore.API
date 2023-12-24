﻿using Application.Common.Data;
using Domain.Orders;
using Domain.Orders.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.GetCustomerOrders;
internal sealed class GetCustomerOrdersQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetCustomerOrdersQuery, Result<List<Order>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<Order>>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context
            .Orders
            .Include(o => o.LineItems)
            .Include(o => o.ShippingInfo)
            .Where(o => o.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);

        if (orders.Count == 0)
        {
            return DomainError.Orders.NotFound;
        }

        return orders;
    }
}