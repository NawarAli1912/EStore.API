using Domain.ModelsSnapshots;
using SharedKernel.Models;

namespace Domain.Products.Events;

public record ProductUpdatedDomainEvent(
    ProductSnapshot Product) : IDomainEvent;

