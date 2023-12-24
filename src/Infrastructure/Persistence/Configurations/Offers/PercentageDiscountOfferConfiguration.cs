using Domain.Offers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Offers;
internal sealed class PercentageDiscountOfferConfiguration
    : IEntityTypeConfiguration<PercentageDiscountOffer>
{
    public void Configure(EntityTypeBuilder<PercentageDiscountOffer> builder)
    {
        builder
          .Property(o => o.Discount)
          .HasPrecision(3, 2);
    }
}
