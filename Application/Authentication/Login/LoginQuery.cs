using Application.Authentication.Common;
using Domain.Kernal;
using MediatR;

namespace Application.Authentication.Login;

public record LoginQuery(string Email, string Password)
    : IRequest<Result<AuthenticationResult>>;
