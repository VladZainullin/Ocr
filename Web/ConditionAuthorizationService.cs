using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;

namespace Web;

internal sealed class ConditionAuthorizationService(
    IAuthorizationService inner,
    IFeatureManager featureManager)
    : IAuthorizationService
{
    public async Task<AuthorizationResult> AuthorizeAsync(
        ClaimsPrincipal user,
        object? resource,
        IEnumerable<IAuthorizationRequirement> requirements)
    {
        if (!await featureManager.IsEnabledAsync("Authorization"))
        {
            return AuthorizationResult.Success();
        }

        return await inner.AuthorizeAsync(user, resource, requirements);
    }

    public async Task<AuthorizationResult> AuthorizeAsync(
        ClaimsPrincipal user,
        object? resource,
        string policyName)
    {
        if (!await featureManager.IsEnabledAsync("Authorization"))
        {
            return AuthorizationResult.Success();
        }

        return await inner.AuthorizeAsync(user, resource, policyName);
    }
}