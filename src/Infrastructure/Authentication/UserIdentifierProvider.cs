using Application.Common.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Authentication;
internal sealed class UserIdentifierProvider : IUserIdentifierProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserIdentifierProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public UserIdentifierProvider(IHttpContextAccessor httpContextAccessor)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)!.Value
                          ?? throw new ArgumentException("The user identifier claim is required.", nameof(httpContextAccessor));

        UserId = new Guid(userIdClaim);
    }

    /// <inheritdoc />
    public Guid UserId { get; }
}
