namespace Infrastructure.Persistence;
public sealed class SqlQueries
{
    public const string ProductsFilterQuery =
        @"WITH ProductsWithRank AS (
        SELECT
            p.""Id"",
            p.""Name"",
            p.""Code"",
            p.""Description"",
            p.""Quantity"",
            p.""PurchasePrice"",
            p.""CustomerPrice"",
            p.""AssociatedOffers"" AS ""AssociatedOffersString"",
            c.""Id"" AS ""CategoryId"",
            c.""Name"" AS ""CategoryName"",
            c.""ParentCategoryId"",
        DENSE_RANK() OVER (ORDER BY p.""Id"") AS ""ProductRank""
        FROM
            ""Products"" p
        JOIN
            ""CategoryProduct"" cp ON p.""Id"" = cp.""ProductsId""
        JOIN
            ""Categories"" c ON cp.""CategoriesId"" = c.""Id""
        WHERE
        (@SearchTerm IS NULL OR p.""Name"" ILIKE '%' || @SearchTerm || '%' OR p.""Description"" ILIKE '%' || @SearchTerm || '%')
        AND (@MinPrice IS NULL OR p.""CustomerPrice"" >= @MinPrice)
        AND (@MaxPrice IS NULL OR p.""CustomerPrice"" <= @MaxPrice)
        AND (@MinQuantity IS NULL OR p.""Quantity"" >= @MinQuantity)
        AND (@MaxQuantity IS NULL OR p.""Quantity"" <= @MaxQuantity)
        AND (
            @OnOffer IS NULL
            OR (
                @OnOffer = TRUE AND p.""AssociatedOffers"" IS NOT NULL AND p.""AssociatedOffers"" <> '[]'
            )
            OR (
                @OnOffer = FALSE AND (p.""AssociatedOffers"" IS NULL OR p.""AssociatedOffers"" = '[]')
            )
        )
        AND (p.""Status"" IN @Statuses))

        SELECT
            ""Id"",
            ""Code"",
            ""Name"",
            ""Description"",
            ""Quantity"",
            ""PurchasePrice"",
            ""CustomerPrice"",
            ""AssociatedOffersString"",
            ""CategoryId"",
            ""CategoryName"",
            ""ParentCategoryId""
        FROM
            ProductsWithRank
        WHERE
            ""ProductRank"" > (@PageIndex-1)*@PageSize AND ""ProductRank"" <= ((@PageIndex-1)*@PageSize) + @PageSize
        ORDER BY
            ""Id""";

    public const string ProductsFilterCount =
        @"
        SELECT
            Count(DISTINCT p.""Id"")
        FROM
            ""Products"" p
	    WHERE
		    (@SearchTerm IS NULL OR p.""Name"" ILIKE '%' || @SearchTerm || '%' OR p.""Description"" ILIKE '%' || @SearchTerm || '%')
		    AND (@MinPrice IS NULL OR p.""CustomerPrice"" >= @MinPrice)
		    AND (@MaxPrice IS NULL OR p.""CustomerPrice"" <= @MaxPrice)
		    AND (@MinQuantity IS NULL OR p.""Quantity"" >= @MinQuantity)
		    AND (@MaxQuantity IS NULL OR p.""Quantity"" <= @MaxQuantity)
		    AND (
			    @OnOffer IS NULL
			    OR (
				    @OnOffer = TRUE AND p.""AssociatedOffers"" IS NOT NULL AND p.""AssociatedOffers"" <> '[]'
			    )
			    OR (
				    @OnOffer = FALSE AND (p.""AssociatedOffers"" IS NULL OR p.""AssociatedOffers"" = '[]')
			    )
		    )
            AND (p.""Status"" IN @Statuses)";

    public const string ProductsCategoryFilter =
        @"
             WITH ProductsWithRank AS (
                 SELECT
                     p.""Id"",
                     p.""Name"",
                     p.""Code"",
                     p.""Description"",
                     p.""Quantity"",
                     p.""PurchasePrice"",
                     p.""CustomerPrice"",
                     p.""AssociatedOffers"" AS ""AssociatedOffersString"",
                     c.""Id"" AS ""CategoryId"",
                     c.""Name"" AS ""CategoryName"",
                     c.""ParentCategoryId"",
             	     DENSE_RANK() OVER (ORDER BY p.""Id"") AS ""ProductRank""
                 FROM
                     ""Products"" p
                 JOIN
                     ""CategoryProduct"" cp ON p.""Id"" = cp.""ProductsId""
                 JOIN
                     ""Categories"" c ON cp.""CategoriesId"" = c.""Id""
                 WHERE
                     cp.""CategoriesId"" IN @CategoryIds)
                 SELECT
                     ""Id"",
                     ""Name"",
                     ""Code"",
                     ""Description"",
                     ""Quantity"",
                     ""PurchasePrice"",
                     ""CustomerPrice"",
                     ""AssociatedOffersString"",
                     ""CategoryId"",
                     ""CategoryName"",
                     ""ParentCategoryId""
                 FROM
                    ProductsWithRank
                 WHERE
                    ""ProductRank"" > (@PageIndex-1)*@PageSize AND ""ProductRank"" <= ((@PageIndex-1)*@PageSize) + @PageSize";

    public const string ProductsCategoryCount =
        @"
        SELECT
            COUNT(DISTINCT p.""Id"")
        FROM
            ""Categories"" c
        LEFT JOIN
            ""CategoryProduct"" cp ON c.""Id"" = cp.""CategoriesId""
        LEFT JOIN
            ""Products"" p ON cp.""ProductsId"" = p.""Id""
        WHERE
            c.""Id"" IN @CategoryIds
        ";
}
