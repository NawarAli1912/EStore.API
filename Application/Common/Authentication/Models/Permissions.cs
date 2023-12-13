namespace Application.Common.Authentication.Models;

[Flags]
public enum Permissions
{
    None = 0,
    ReadDetails = 1,
    ManageProducts = 2,
    ManageCategories = 4,
    ManageCarts = 8,
    ManageOrders = 16,
    ManageCustomers = 32,
    ManageRoles = 64,
    ConfigureAccessControl = 128,
    ManageOrdersLite = 256,
    All = ~None
}
