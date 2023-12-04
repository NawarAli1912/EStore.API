using Application.Common.Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication;
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permissions permission)
        : base(permission.ToString())
    {
        Permissions = permission;
    }

    public Permissions Permissions
    {
        get
        {
            return !string.IsNullOrEmpty(Policy)
                ? PolicyNameHelper.GetPermissionsFrom(Policy)
                : Permissions.None;
        }
        set
        {
            Policy = value != Permissions.None
                ? PolicyNameHelper.GeneratePolicyNameFor(value)
                : string.Empty;
        }
    }
}
