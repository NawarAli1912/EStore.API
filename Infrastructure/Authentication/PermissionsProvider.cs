using Application.Common.Authentication.Models;

namespace Infrastructure.Authentication;
public static class PermissionsProvider
{
    public static List<Permissions> GetAll()
    {
        return Enum
                .GetValues(typeof(Permissions))
                .Cast<Permissions>()
                .ToList();
    }
}
