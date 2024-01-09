using Application.Common.Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication.Authorization;
public sealed class PermissionRequirement(Permissions permission)
        : IAuthorizationRequirement
{
    public Permissions Permission { get; } = permission;
}
