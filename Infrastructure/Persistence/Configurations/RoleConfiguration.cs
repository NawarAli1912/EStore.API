using Application.Common.Authentication.Models;
using Domain.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasMany(r => r.Permissions)
            .WithMany();

        builder.HasData(
                Enum.GetValues<Roles>()
                .Select(r => new Role(r.ToString())
                {
                    NormalizedName = r.ToString().ToUpperInvariant(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }));
    }
}
