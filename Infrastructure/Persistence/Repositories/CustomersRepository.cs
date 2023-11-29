using Domain.Customers;
using Domain.Repositories;

namespace Infrastructure.Persistence.Repositories;

internal class CustomersRepository : ICustomersRepository
{
    private readonly ApplicationDbContext _context;

    public CustomersRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Customer customer)
    {
        await _context
            .Customers
            .AddAsync(customer);

        await _context.SaveChangesAsync();
    }


}
