using Domain.Customers;

namespace Domain.Repositories;
public interface ICustomersRepository
{
    Task Create(Customer customer);
}
