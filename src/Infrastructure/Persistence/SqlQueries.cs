namespace Infrastructure.Persistence;
public sealed class SqlQueries
{
    public const string ProductsFilterQuery =
        @"WITH ProductsWithRank AS (
        SELECT
            p.Id,
            p.Name,
            p.Code,
            p.Description,
            p.Quantity,
            p.PurchasePrice,
            p.CustomerPrice,
            p.AssociatedOffers,
            c.Id AS CategoryId,
            c.Name AS CategoryName,
            c.ParentCategoryId,
        DENSE_RANK() OVER (ORDER BY p.Id) AS ProductRank
        FROM
            Products p
        JOIN
            CategoryProduct cp ON p.Id = cp.ProductsId
        JOIN
            Categories c ON cp.CategoriesId = c.Id
        WHERE 
        (@SearchTerm IS NULL OR P.Name LIKE '%' + @SearchTerm + '%' OR P.Description LIKE '%' + @SearchTerm + '%')
        AND (@MinPrice IS NULL OR P.CustomerPrice >= @MinPrice)
        AND (@MaxPrice IS NULL OR P.CustomerPrice <= @MaxPrice)
        AND (@MinQuantity IS NULL OR P.Quantity >= @MinQuantity)
        AND (@MaxQuantity IS NULL OR P.Quantity <= @MaxQuantity)
        AND (
            @OnOffer IS NULL
            OR (
                @OnOffer = 1 AND P.AssociatedOffers IS NOT NULL AND JSON_QUERY(P.AssociatedOffers) <> '[]'
            )
            OR (
                @OnOffer = 0 AND (P.AssociatedOffers IS NULL OR JSON_QUERY(P.AssociatedOffers) = '[]')
            )
        )
        AND (p.Status IN @Stasuses))

        SELECT
            Id,
            Code,
            Name,
            Description,
            Quantity,
            PurchasePrice,
            CustomerPrice,
            AssociatedOffers,
            CategoryId,
            CategoryName,
            ParentCategoryId
        FROM 
            ProductsWithRank
        WHERE 
            ProductRank > (@PageIndex-1)*@PageIndex AND ProductRank <= ((@PageIndex-1)*@PageIndex) + @PageSize
        ORDER BY 
            Id";

    public const string ProductsFilterCount =
        @"
        SELECT
            Count(DISTINCT p.Id)
        FROM
            Products p
	    WHERE 
		    (@SearchTerm IS NULL OR P.Name LIKE '%' + @SearchTerm + '%' OR P.Description LIKE '%' + @SearchTerm + '%')
		    AND (@MinPrice IS NULL OR P.CustomerPrice >= @MinPrice)
		    AND (@MaxPrice IS NULL OR P.CustomerPrice <= @MaxPrice)
		    AND (@MinQuantity IS NULL OR P.Quantity >= @MinQuantity)
		    AND (@MaxQuantity IS NULL OR P.Quantity <= @MaxQuantity)
		    AND (
			    @OnOffer IS NULL
			    OR (
				    @OnOffer = 1 AND P.AssociatedOffers IS NOT NULL AND JSON_QUERY(P.AssociatedOffers) <> '[]'
			    )
			    OR (
				    @OnOffer = 0 AND (P.AssociatedOffers IS NULL OR JSON_QUERY(P.AssociatedOffers) = '[]')
			    )
		    )
            AND (p.Status IN @Stasuses)";

    public const string ProductsCategoryFilter =
        @"
             WITH ProductsWithRank AS (
                 SELECT
                     p.Id,
                     p.Name,
                     p.Code,
                     p.Description,
                     p.Quantity,
                     p.PurchasePrice,
                     p.CustomerPrice,
                     p.AssociatedOffers,
                     c.Id AS CategoryId,
                     c.Name AS CategoryName,
                     c.ParentCategoryId,
             	     DENSE_RANK() OVER (ORDER BY p.Id) AS ProductRank
                 FROM
                     Products p
                 JOIN
                     CategoryProduct cp ON p.Id = cp.ProductsId
                 JOIN
                     Categories c ON cp.CategoriesId = c.Id
                 WHERE
                     cp.CategoriesId IN @CategoryIds)
                 SELECT
                     Id,
                     Name,
                     Code,
                     Description,
                     Quantity,
                     PurchasePrice,
                     CustomerPrice,
                     AssociatedOffers,
                     CategoryId,
                     CategoryName,
                     ParentCategoryId
                 FROM 
                    ProductsWithRank
                 WHERE 
                    ProductRank > (@PageIndex-1)*@PageIndex AND ProductRank <= ((@PageIndex-1)*@PageIndex) + @PageSize";

    public const string ProductsCategoryCount =
        @"
        SELECT
            COUNT(DISTINCT p.Id)
        FROM
            Categories c
        LEFT JOIN
            CategoryProduct cp ON c.Id = cp.CategoriesId
        LEFT JOIN
            Products p ON cp.ProductsId = p.Id
        WHERE
            c.Id IN @CategoryIds
        ";
}
