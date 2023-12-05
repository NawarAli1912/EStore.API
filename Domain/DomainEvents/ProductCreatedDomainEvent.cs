using Domain.Kernal.Models;
using Domain.Products;

namespace Domain.DomainEvents;
public sealed record ProductCreatedDomainEvent(Product Product) : IDomainEvent;