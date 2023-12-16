using Application.Common.Authentication.Models;
using Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissionClaim = context.User.Claims.FirstOrDefault(
            c => c.Type == CustomClaims.Permissions);

        if (permissionClaim == null)
        {
            return Task.CompletedTask;
        }

        if (!int.TryParse(permissionClaim.Value, out var permissionClaimValue))
        {
            return Task.CompletedTask;
        }

        var userPermissions = (Permissions)permissionClaimValue;
        if ((userPermissions & requirement.Permissions) != 0)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
