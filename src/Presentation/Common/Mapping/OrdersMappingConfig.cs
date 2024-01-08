using Application.Orders.List;
using Application.Orders.Update;
using Contracts.Orders;
using Domain.Orders;
using Domain.Orders.Entities;
using Domain.Orders.ValueObjects;
using Mapster;
using Microsoft.CodeAnalysis;

namespace Presentation.Common.Mapping;

public class OrdersMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<OrderStatus, Domain.Orders.Enums.OrderStatus>()
            .MapWith(src => MapToDomainOrderStatus(src));

        config.NewConfig<ListOrdersFilter, OrdersFilter>()
            .MapWith(src => MapListOrdersFilterToOrdersFilter(src));

        config.NewConfig<ShippingInfo, ShippingInfoResponse>();

        config.NewConfig<Order, OrderResponse>()
            .Map(dest => dest.LineItems, src => MapLineItemsToLineItemsResponse(src.LineItems.ToList()));

        config.NewConfig<ShippingInfoRequest, UpdateShippingInfo>();

        config.NewConfig<(Guid, UpdateOrderRequest), UpdateOrderCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);
    }

    private List<OrderItemResponse> MapLineItemsToLineItemsResponse(List<LineItem> src)
    {
        List<OrderItemResponse> result = [];

        var groups = src.GroupBy(item => item.ProductId);
        foreach (var group in groups)
        {
            result.Add(new OrderItemResponse(
                group.Key,
                group.Count(),
                group.Select(item => item.Price).Sum()));
        }

        return result;

    }

    private static OrdersFilter MapListOrdersFilterToOrdersFilter(ListOrdersFilter src)
    {
        List<Domain.Orders.Enums.OrderStatus> status = [];
        if (src.Status is null || !src.Status.Any())
        {
            status = Enum
                    .GetValues(typeof(Domain.Orders.Enums.OrderStatus))
                    .Cast<Domain.Orders.Enums.OrderStatus>().ToList();
        }
        else
        {
            foreach (var s in src.Status)
            {
                status.Add(MapToDomainOrderStatus(s));
            }
        }

        return new OrdersFilter(status, src.ModifiedFrom ?? DateTime.MinValue, src.ModifiedTo ?? DateTime.MaxValue);
    }

    private static Domain.Orders.Enums.OrderStatus MapToDomainOrderStatus(OrderStatus src) => src switch
    {
        OrderStatus.Approved => Domain.Orders.Enums.OrderStatus.Approved,
        OrderStatus.Rejected => Domain.Orders.Enums.OrderStatus.Rejected,
        OrderStatus.Pending => Domain.Orders.Enums.OrderStatus.Pending,
        OrderStatus.Shipped => Domain.Orders.Enums.OrderStatus.Shipped,
        _ => throw new ArgumentException()
    };
}
