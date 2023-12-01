using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Presentation.Controllers;


[Route("roles")]
public class RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) : ApiController
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [HttpPost("create")]
    public async Task<IActionResult> AddRole(string roleName)
    {
        if (await _roleManager.FindByNameAsync(roleName) is not null)
            throw new ArgumentException();

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
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
}
