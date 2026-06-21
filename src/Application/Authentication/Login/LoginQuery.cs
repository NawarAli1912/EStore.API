using Application.Authentication.Common;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Authentication.Login;

public record LoginQuery(string Email, string Password)
    : IRequest<Result<AuthenticationResult>>;
