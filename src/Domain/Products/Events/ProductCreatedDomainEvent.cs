using SharedKernel.Primitives;

namespace Domain.Products.Events;
public sealed record ProductCreatedDomainEvent(Product Product)
    : IDomainEvent;