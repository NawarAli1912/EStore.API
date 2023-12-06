namespace Application.Common.Repository;

public interface ICategoriesRepository
{
    Task<List<Guid>> GetCategoryIdsInHierarchy(Guid categoryId);
}
