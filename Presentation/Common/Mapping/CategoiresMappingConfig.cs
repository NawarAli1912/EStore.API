using Application.Categories.Update;
using Contracts.Categories;
using Mapster;

namespace Presentation.Common.Mapping;

public class CategoiresMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(Guid, UpdateCategoryRequest), UpdateCategoryCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);
    }
}
