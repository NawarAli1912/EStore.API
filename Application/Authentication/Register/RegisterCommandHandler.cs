using Application.Authentication.Common;
using Application.Common.Authentication;
using Application.Common.Authentication.Jwt;
using Application.Common.Data;
using Domain.Customers;
using Domain.Customers.ValueObjects;
using Domain.DomainErrors.Customers;
using Domain.Kernal;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Register;

internal class RegisterCommandHandler(
    UserManager<IdentityUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IApplicationDbContext context) : IRequestHandler<RegisterCommand, Result<AuthenticationResult>>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<AuthenticationResult>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
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

        var roles = new List<string>
        {
            Roles.Customer
        };

        await _userManager.AddToRolesAsync(user, roles);

        var token = _jwtTokenGenerator.Generate(user, roles);

        var address = Address.Create(
            request.City,
            request.Country,
            request.PostalCode,
            request.Builing,
            request.Street);

        var customer = Customer.Create(userId, address);

        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            userId.ToString(),
            user.UserName,
            user.Email,
            token);
    }
}
