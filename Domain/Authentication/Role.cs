using Microsoft.AspNetCore.Identity;

namespace Domain.Authentication;
public sealed class Role(string name)
    : IdentityRole(name)
{
    public ICollection<Permission> Permissions { get; set; } = [];
}
