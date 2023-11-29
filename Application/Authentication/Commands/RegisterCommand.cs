using Domain.Kernal;
using MediatR;

namespace Application.Authentication.Commands;

public record RegisterCommand(
    string UserName,
    string Email,
    string Password,
    string? Street,
    string? Builing,
    string? City,
    string? Country,
    string? PostalCode) : IRequest<Result<string>>;
