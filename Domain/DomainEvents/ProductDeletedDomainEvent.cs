using Domain.Kernal.Models;

namespace Domain.DomainEvents;

public record ProductDeletedDomainEvent(Guid Id)
    : IDomainEvent;
