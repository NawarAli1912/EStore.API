using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;
public sealed class PermissionAuthroizationPolicyProvider(IOptions<AuthorizationOptions> options)
                : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _options = options.Value;

    public async override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy == null && PolicyNameHelper.IsValidPolicyName(policyName))
        {
            var permissions = PolicyNameHelper.GetPermissionsFrom(policyName);

            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permissions))
                .Build();

            _options.AddPolicy(policyName!, policy);
        }

        return policy;
    }
}
