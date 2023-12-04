using Application.Common.Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authentication;

public sealed class HasPermissionAttribute(Permissions permission) :
    AuthorizeAttribute(permission.ToString())
{
}
