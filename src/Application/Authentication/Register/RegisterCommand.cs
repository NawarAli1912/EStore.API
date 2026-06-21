using Application.Authentication.Common;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Authentication.Register;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password) : IRequest<Result<AuthenticationResult>>;
