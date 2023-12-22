using Domain.ModelsSnapshots;
using SharedKernel.Primitives;

namespace Domain.Products.Events;

public record ProductUpdatedDomainEvent(
    ProductSnapshot Product) : IDomainEvent;

