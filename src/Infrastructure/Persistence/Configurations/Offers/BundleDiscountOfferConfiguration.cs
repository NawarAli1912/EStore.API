using Domain.Offers;
using Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Offers;
internal class BundleDiscountOfferConfiguration : IEntityTypeConfiguration<BundleDiscountOffer>
{
    public void Configure(EntityTypeBuilder<BundleDiscountOffer> builder)
    {
        builder.Property<List<Guid>>("_bundleProductsIds")
            .HasColumnName("ProductsBundle")
            .HasListOfIdsConverter();

        builder
          .Property(o => o.Discount)
          .HasPrecision(3, 2);
    }
}
