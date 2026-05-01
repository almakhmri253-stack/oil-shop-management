using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OilShopManagement.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // Admin ظٹظ…ظ„ظƒ ظƒظ„ ط§ظ„طµظ„ط§ط­ظٹط§طھ طھظ„ظ‚ط§ط¦ظٹط§ظ‹
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // طھط­ظ‚ظ‚ ظ…ظ† ط§ظ„ظ€ Claims
        if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == requirement.Permission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public static class PolicyNames
{
    public static string For(string permission) => $"Permission:{permission}";
}


