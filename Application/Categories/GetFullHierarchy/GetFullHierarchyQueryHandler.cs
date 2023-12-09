using Application.Common.Data;
using Dapper;
using Domain.Categories;
using MediatR;
using Microsoft.Data.SqlClient;

namespace Application.Categories.GetFullHierarchy;

public sealed class GetFullHierarchyQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IRequestHandler<GetFullHierarchyQuery, List<Category>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<List<Category>> Handle(GetFullHierarchyQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection sqlConnection = _sqlConnectionFactory.Create();

        var sql = @"SELECT * FROM Categories";

        var queryResult = (await sqlConnection.QueryAsync<Category>(sql)).ToList();

        var rootCategories = queryResult
                .Where(c => c.ParentCategoryId is null).ToList();

        var result = rootCategories
            .Select(root => Category.Create(
                root.Id,
                root.Name,
                null!,
                Category.BuildCategoryTree(queryResult, root.Id),
                root.ParentCategoryId))
            .ToList();

        return [.. result];
    }
}
