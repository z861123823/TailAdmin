using System.Security.Claims;
using TailAdmin.Templates.Rcl.Security;

namespace TailAdmin.Demo.Web.Infrastructure;

/// <summary>演示宿主的当前用户实现：从 Cookie 身份 Claims 读取。</summary>
public sealed class DemoCurrentAdmin : ICurrentAdmin
{
    private readonly IHttpContextAccessor _accessor;

    /// <summary>初始化当前用户。</summary>
    /// <param name="accessor">HTTP 上下文访问器。</param>
    public DemoCurrentAdmin(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    /// <inheritdoc />
    public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    /// <inheritdoc />
    public string? UserName => _accessor.HttpContext?.User.Identity?.Name;

    /// <inheritdoc />
    public string? DisplayName => _accessor.HttpContext?.User.FindFirst("display_name")?.Value;

    /// <inheritdoc />
    public IReadOnlyCollection<string> Permissions
    {
        get
        {
            var user = _accessor.HttpContext?.User;
            if (user is null)
            {
                return Array.Empty<string>();
            }

            return user.FindAll("perm").Select(claim => claim.Value).ToArray();
        }
    }
}

/// <summary>演示宿主的 Claims 常量。</summary>
public static class DemoClaims
{
    /// <summary>显示名。</summary>
    public const string DisplayName = "display_name";

    /// <summary>权限码。</summary>
    public const string Permission = "perm";
}
