namespace Application.Common.Authentication.Models;

[Flags]
public enum Permissions
{
    None = 0,
    ReadDetails = 1,
    CreateProduct = 2,
    ConfigureAccessControl = 4,
    ViewRoles = 8,
    ManageRoles = 16,
    All = ~None
}
