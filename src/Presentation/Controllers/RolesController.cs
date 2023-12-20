using Application.Common.Authentication.Models;
using Domain.Authentication;
using Infrastructure.Authentication.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Presentation.Controllers;


[Route("api/roles")]
[HasPermission(Permissions.ConfigureAccessControl)]
public class RolesController(RoleManager<Role> roleManager, UserManager<IdentityUser> userManager)
    : ApiController
{
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [HttpPost("create")]
    public async Task<IActionResult> AddRole(string roleName)
    {
        if (await _roleManager.FindByNameAsync(roleName) is not null)
            throw new ArgumentException();

        var result = await _roleManager.CreateAsync(new Role(roleName)
        {
            NormalizedName = roleName.ToUpper(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    [HttpPost("assign-user")]
    public async Task<IActionResult> AssignUserToRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var role = await _roleManager.FindByNameAsync(roleName);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var result = await _userManager.AddToRoleAsync(user!, role.Name!);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        return Ok(result);
    }

    [HttpPost("add-permission")]
    public async Task<IActionResult> AssignPermissionToRole(string roleName, Permissions permission)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        role!.Permissions.Add(new Permission
        {
            Name = permission.ToString()
        });

        var result = await _roleManager.UpdateAsync(role);

        return Ok(result);
    }
}
