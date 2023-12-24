using Domain.Offers;
using Domain.Offers.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Offers;

internal class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder
            .ToTable(TablesNames.Offer);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.
            Property(o => o.Description)
            .HasMaxLength(512);

        builder.HasDiscriminator(o => o.Type)
          .HasValue<Offer>(OfferType.None)
          .HasValue<PercentageDiscountOffer>(OfferType.PercentageDiscountOffer) // derived type
          .HasValue<BundleDiscountOffer>(OfferType.BundleDiscountOffer); // derived type
    }
}
