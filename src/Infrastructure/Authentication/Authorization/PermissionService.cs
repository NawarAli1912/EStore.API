using Application.Common.Authentication.Models;
using Domain.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication.Authorization;
public sealed class PermissionService(UserManager<IdentityUser> userManager, RoleManager<Role> roleManager) : IPermissionService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;

    public async Task<int> GetPermissions(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var rolesNames = await _userManager.GetRolesAsync(user!);

        var roles = await _roleManager
                    .Roles
                    .Include(r => r.Permissions)
                    .Where(r => rolesNames.Contains(r.Name!))
                    .ToListAsync();

        return roles
        .SelectMany(r => r.Permissions)
        .Aggregate(0, (current, permission) => current | (int)Enum.Parse(
            typeof(Permissions),
            permission.Name));
    }
}
