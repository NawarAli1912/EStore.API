using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Dapper;

namespace Infrastructure.Persistence.Repository;
public sealed class CategoriesRepository(ISqlConnectionFactory sqlConnectionFactory) : ICategoriesRepository
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<List<Guid>> GetCategoryIdsInHierarchy(Guid categoryId)
    {
        await using var connection = _sqlConnectionFactory.Create();
        var categoryIds = new List<Guid> { categoryId };
        var subcategoryIds = (await connection
          .QueryAsync<Guid>("""
                            WITH RecursiveCategoryCTE AS (
                               SELECT
                                  Id
                               FROM
                                  Categories
                               WHERE
                                  Id = @CategoryId
    
                               UNION ALL
    
                               SELECT
                                  c.Id
                               FROM
                                  Categories c
                               INNER JOIN
                                  RecursiveCategoryCTE r ON c.ParentCategoryId = r.Id
                              )
                              SELECT Id FROM RecursiveCategoryCTE
                            """,
                          new { CategoryId = categoryId }))
                        .ToList();
        categoryIds.AddRange(subcategoryIds);
        return categoryIds;
    }
}
