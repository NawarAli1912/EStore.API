using FluentValidation;

namespace Application.Authentication.Register;
internal class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.UserName)
               .NotEmpty().WithMessage("Username is required.")
               .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 6 characters.");
    }
}
