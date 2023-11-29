using Domain.Kernal;
using MediatR;

namespace Application.Authentication.Commands;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password) : IRequest<Result<string>>;
