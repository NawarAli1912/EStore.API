namespace Application.Common.Authentication.Models;

[Flags]
public enum Permissions
{
    None = 0,
    ReadDetails = 1 << 1,
    ManageProducts = 1 << 2,
    ManageCategories = 1 << 3,
    ManageCarts = 1 << 4,
    ManageOrders = 1 << 5,
    ManageCustomers = 1 << 6,
    ManageRoles = 1 << 7,
    ConfigureAccessControl = 1 << 8,
    All = ~None
}
