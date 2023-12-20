namespace Infrastructure.Authentication.Authorization;

public interface IPermissionService
{
    Task<int> GetPermissions(string userId);
}
