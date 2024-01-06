using Infrastructure.Persistence.FriendlyIdentifiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;
internal sealed class FriendlyIdSequenceConfiguration : IEntityTypeConfiguration<FriendlyIdSequence>
{
    public void Configure(EntityTypeBuilder<FriendlyIdSequence> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasData(new FriendlyIdSequence
        {
            ProductSequence = 2024,
            OrderSequence = 4202
        });
    }
}
