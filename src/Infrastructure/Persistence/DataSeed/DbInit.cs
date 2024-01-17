using Application.Common.Authentication.Models;
using Domain.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.DataSeed;

public class DbInit
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public DbInit(UserManager<IdentityUser> userManager, RoleManager<Role> roleManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task Init()
    {
        if (!await _userManager.Users.AnyAsync())
        {
            var permissions = await _dbContext.Permissions.ToListAsync();
            var adminRole = new Role("Admin")
            {
                Id = Guid.NewGuid().ToString(),
                NormalizedName = "Admin".ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Permissions = new List<Permission>()
            {
                permissions.First(p => p.Name == Permissions.All.ToString())
            }
            };
            var customerRole = new Role("Customer")
            {
                Id = Guid.NewGuid().ToString(),
                NormalizedName = "Customer".ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Permissions = new List<Permission>()
            {
                permissions.First(p => p.Name == Permissions.ManageCarts.ToString()),
                permissions.First(p => p.Name == Permissions.ManageOrdersLite.ToString())
            }
            };

            await _roleManager.CreateAsync(adminRole);
            await _roleManager.CreateAsync(customerRole);

            var admin = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "SuperUser",
                Email = "admin@estore.com",
                EmailConfirmed = true,
                NormalizedEmail = "admin@estore.com".ToUpper(),
            };

            await _userManager.CreateAsync(admin, "estoreadmin");
            await _userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
