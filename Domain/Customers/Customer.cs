using Domain.Kernal.Models;

namespace Domain.Customers;

public class Customer : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    private Customer() : base(Guid.NewGuid())
    {
    }
}
