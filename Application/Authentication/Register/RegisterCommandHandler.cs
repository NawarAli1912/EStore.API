using Application.Common.Authentication.Jwt;
using Application.Common.Data;
using Domain.Customers;
using Domain.Customers.ValueObjects;
using Domain.Errors.Customers;
using Domain.Kernal;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Register;

internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(
        UserManager<IdentityUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
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

        await _context.Customers.AddAsync(customer, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return _jwtTokenGenerator.Generate(user.Id, user.UserName, user.Email);
    }
}
