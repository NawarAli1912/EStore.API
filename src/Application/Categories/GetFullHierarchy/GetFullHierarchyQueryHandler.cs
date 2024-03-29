﻿using Application.Common.DatabaseAbstraction;
using Dapper;
using Domain.Categories;
using Domain.Errors;
using MediatR;
using Microsoft.Data.SqlClient;
using SharedKernel.Primitives;

namespace Application.Categories.GetFullHierarchy;

public sealed class GetFullHierarchyQueryHandler
    : IRequestHandler<GetFullHierarchyQuery, Result<List<Category>>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetFullHierarchyQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<List<Category>>> Handle(GetFullHierarchyQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection sqlConnection = _sqlConnectionFactory.Create();

        var sql = @"SELECT * FROM Categories";

        var queryResult = (await sqlConnection.QueryAsync<Category>(sql)).ToList();

        var rootCategories = queryResult
                .Where(c => c.ParentCategoryId is null).ToList();

        if (rootCategories is null || rootCategories.Count == 0)
        {
            return DomainError.Categories.NotFound;
        }

        var result = rootCategories
            .Select(root => Category.Create(
                root.Id,
                root.Name,
                root.ParentCategoryId,
                null!,
                Category.BuildCategoryTree(queryResult, root.Id)))
            .ToList();

        return result;
    }
}
