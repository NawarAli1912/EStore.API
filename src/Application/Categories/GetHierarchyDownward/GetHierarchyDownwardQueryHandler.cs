using Application.Common.Data;
using Dapper;
using Domain.Categories;
using Domain.Categories.Errors;
using MediatR;
using Microsoft.Data.SqlClient;
using SharedKernel.Primitives;

namespace Application.Categories.GetHierarchyDownward;

public sealed class GetHierarchyDownwardQueryHandler(
            ISqlConnectionFactory sqlConnectionFactory) :
        IRequestHandler<GetHierarchyDownwardQuery, Result<Category>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<Result<Category>> Handle(
        GetHierarchyDownwardQuery request,
        CancellationToken cancellationToken)
    {
        await using SqlConnection sqlConnection = _sqlConnectionFactory.Create();

        var sql = @"
                WITH RecursiveCategoryCTE AS (
                    SELECT
                        Id,
                        Name,
                        ParentCategoryId
                    FROM
                        Categories
                    WHERE
                        Id = @RootCategoryId
                    UNION ALL
                    SELECT
                        c.Id,
                        c.Name,
                        c.ParentCategoryId
                    FROM
                        Categories c
                    INNER JOIN
                        RecursiveCategoryCTE r ON c.ParentCategoryId = r.Id
                )
                SELECT
                    Id,
                    Name,
                    ParentCategoryId
                FROM
                    RecursiveCategoryCTE
                ";

        var queryResult = await sqlConnection
                .QueryAsync<Category>(sql, new { RootCategoryId = request.Id });

        var categories = queryResult.ToList();
        if (categories.Count == 0)
        {
            return DomainError.Category.NotFound;
        }

        return Category.BuildCategoryTree(
            categories,
            categories
                .First(c => c.Id == request.Id).ParentCategoryId)
                .First();


    }
}
