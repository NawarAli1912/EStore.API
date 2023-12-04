using Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication;

public sealed class HasPermissionAttribute(Permission permission) :
    AuthorizeAttribute(permission.ToString())
{
}
