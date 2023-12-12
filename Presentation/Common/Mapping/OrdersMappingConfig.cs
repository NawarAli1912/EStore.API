using Application.Orders.List;
using Contracts.Orders;
using Mapster;

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
