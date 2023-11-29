using Application.Authentication.Common;
using Domain.Kernal;
using MediatR;

namespace Application.Authentication.Register;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password,
    string? Street,
    string? Builing,
    string? City,
    string? Country,
    string? PostalCode) : IRequest<Result<AuthenticationResult>>;
