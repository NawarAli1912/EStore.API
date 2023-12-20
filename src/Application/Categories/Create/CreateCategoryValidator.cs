using FluentValidation;

namespace Application.Categories.Create;
public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Name)
            .Length(2, 32);
    }
}
