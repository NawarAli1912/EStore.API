namespace Infrastructure.Authentication;

public interface IPermissionService
{
    Task<int> GetPermissions(string userId);
}
