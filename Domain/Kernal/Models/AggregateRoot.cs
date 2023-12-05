namespace Domain.Kernal.Models;

public abstract class AggregateRoot<T> : Entity<T>
    where T : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(T id) : base(id)
    {
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
