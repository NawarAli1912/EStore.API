using Application.Common.Authentication.Models;
using Domain.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasIndex(x => x.Id);

        builder.HasData(
            Enum.GetValues<Permissions>()
            .Select((p, index) => new Permission
            {
                Id = -(index + 1),
                Name = p.ToString()
            }));
    }
}
