using Application.Authentication.Common;
using Domain.Kernal;
using MediatR;

namespace Application.Authentication.Register;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password) : IRequest<Result<AuthenticationResult>>;
