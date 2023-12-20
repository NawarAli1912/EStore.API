using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication.Authorization;
public sealed class PermissionAuthroizationPolicyProvider(IOptions<AuthorizationOptions> options)
                : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _options = options.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy is not null || !PolicyNameHelper.IsValidPolicyName(policyName))
        {
            return policy;
        }

        var permissions = PolicyNameHelper.GetPermissionsFrom(policyName);

        policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(permissions))
            .Build();

        _options.AddPolicy(policyName!, policy);

        return policy;
    }
}
