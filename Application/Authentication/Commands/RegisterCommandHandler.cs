using Application.Common.Authentication.Jwt;
using Domain.Customers;
using Domain.Customers.ValueObjects;
using Domain.Errors.Customers;
using Domain.Kernal;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Commands;

internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICustomersRepository _customersRepository;

    public RegisterCommandHandler(
        UserManager<IdentityUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        ICustomersRepository customersRepository)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _customersRepository = customersRepository;
    }

    public async Task<Result<string>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        if (_userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Errors.Customers.DuplicateEmail;
        }
        var userId = Guid.NewGuid();

        var user = new IdentityUser
        {
            Id = userId.ToString(),
            UserName = request.UserName,
            Email = request.Email,
        };


        var userCreatedResult = await _userManager.CreateAsync(user, request.Password);

        if (!userCreatedResult.Succeeded)
        {
            return Error.Unexpected(code: userCreatedResult.Errors.First().Code);
        }

        var address = Address.Create(
            request.City,
            request.Country,
            request.PostalCode,
            request.Builing,
            request.Street);

        var customer = Customer.Create(userId, address);

        await _customersRepository.Create(customer);
        return _jwtTokenGenerator.Generate(user.Id, user.UserName, user.Email);
    }
}
