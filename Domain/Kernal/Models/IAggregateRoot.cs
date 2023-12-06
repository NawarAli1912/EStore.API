namespace Domain.Kernal.Models;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    void RaiseDomainEvent(IDomainEvent domainEvent);

    void ClearDomainEvent();
}
