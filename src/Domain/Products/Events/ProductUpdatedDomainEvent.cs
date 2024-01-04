using SharedKernel.Primitives;

namespace Domain.Products.Events;

public sealed record ProductUpdatedDomainEvent(
    Product Product) : IDomainEvent;

