using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRoleAttribute : Attribute, IAsyncActionFilter
{
    private readonly string[] _roles;
    public RequireRoleAttribute(params string[] roles) => _roles = roles;

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claims = context.HttpContext.Items["claims"] as IDictionary<string, object>;
        if (claims == null || !claims.TryGetValue("role", out var roleObj))
        {
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }

        var role = roleObj?.ToString() ?? string.Empty;
        if (!_roles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }

        return next();
    }
}
