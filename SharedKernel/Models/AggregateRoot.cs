namespace SharedKernel.Models;

public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot
    where T : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(T id) : base(id)
    {
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.ToList();

    public void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvent() => _domainEvents.Clear();
}
