using Domain.Kernal.Models;
using Domain.ModelsSnapshots;

namespace Domain.DomainEvents;
public sealed record ProductCreatedDomainEvent(ProductSnapshot Product) : IDomainEvent;