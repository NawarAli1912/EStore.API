using FluentValidation;

namespace Application.Authentication.Login;

public sealed class LoginQueryValidato : AbstractValidator<LoginQuery>
{
    public LoginQueryValidato()
    {
        RuleFor(query => query.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(query => query.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 6 characters.");
    }
}
