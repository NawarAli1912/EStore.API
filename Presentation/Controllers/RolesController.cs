using Application.Common.Authentication.Models;
using Domain.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Presentation.Controllers;


[Route("roles")]
public class RolesController : ApiController
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RolesController(RoleManager<Role> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddRole(string roleName)
    {
        if (await _roleManager.FindByNameAsync(roleName) is not null)
            throw new ArgumentException();

        var result = await _roleManager.CreateAsync(new Role(roleName));
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AssignUserToRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var role = await _roleManager.FindByNameAsync(roleName);

        var result = await _userManager.AddToRoleAsync(user!, role.Name!);

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
