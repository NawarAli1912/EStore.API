using Domain.ModelsSnapshots;
using SharedKernel.Primitives;

namespace Domain.Products.Events;
public sealed record ProductCreatedDomainEvent(ProductSnapshot Product)
    : IDomainEvent;