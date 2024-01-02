using Application.Categories.Update;
using Contracts.Categories;
using Domain.Categories;
using Mapster;
using SubcategoryActions = Contracts.Categories.SubcategoryActions;

namespace Presentation.Common.Mapping;

public class CategoiresMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SubcategoryActions, Domain.Categories.Enums.SubcategoryActions>();

        config.NewConfig<(Guid, UpdateCategoryRequest), UpdateCategoryCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<Category, CategoryResponse>();
    }
}
