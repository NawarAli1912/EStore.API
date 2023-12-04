using Application.Common.Authentication.Models;
using Domain.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasData(
            Enum.GetValues<Permissions>()
            .Select(p => new Permission
            {
                Id = (int)p,
                Name = p.ToString()
            }));
    }
}
