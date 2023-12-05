using Application.Authentication.Common;
using Application.Common.Authentication.Jwt;
using Application.Common.Authentication.Models;
using Application.Common.Data;
using Domain.Customers;
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

        string token;
        var userId = Guid.NewGuid();
        var user = new IdentityUser
        {
            Id = userId.ToString(),
            UserName = request.UserName,
            Email = request.Email,
        };

        await _context.BeginTransactionAsync();
        try
        {
            await _userManager.CreateAsync(user, request.Password);
            await _userManager.AddToRoleAsync(user, Roles.Customer.ToString());

            token = await _jwtTokenGenerator.Generate(user);

            var customer = Customer.Create(userId);

            await _context.Customers.AddAsync(customer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.CommitTransactionAsync();
        }
        catch
        {
            await _context.RollbackTransactionAsync();
            return Error.Unexpected();
        }

        return new AuthenticationResult(
            userId.ToString(),
            user.UserName,
            user.Email,
            token);
    }
}
