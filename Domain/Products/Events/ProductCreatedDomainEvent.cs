using Domain.ModelsSnapshots;
using SharedKernel.Models;

namespace Domain.Products.Events;
public sealed record ProductCreatedDomainEvent(ProductSnapshot Product)
    : IDomainEvent;