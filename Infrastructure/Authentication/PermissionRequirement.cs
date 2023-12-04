using Application.Common.Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication;
public sealed class PermissionRequirement(Permissions permission)
        : IAuthorizationRequirement
{
    public Permissions Permissions { get; } = permission;
}
