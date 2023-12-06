using Domain.Kernal.Models;
using Domain.ModelsSnapshots;

namespace Domain.DomainEvents;

public record ProductUpdatedDomainEvent(
    ProductSnapshot Product) : IDomainEvent;

