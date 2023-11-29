using Domain.Customers;

namespace Domain.Repositories;
public interface ICusotmersRepository
{
    Task Create(Customer customer);
}
