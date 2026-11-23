using TailAdmin.Templates.Rcl.Security;

namespace TailAdminHost.Infrastructure;

/// <summary>从 Cookie Claims 读取当前登录用户。</summary>
public sealed class TemplateCurrentAdmin : ICurrentAdmin
{
    private readonly IHttpContextAccessor _accessor;

    /// <summary>初始化当前用户。</summary>
    /// <param name="accessor">HTTP 上下文访问器。</param>
    public TemplateCurrentAdmin(IHttpContextAccessor accessor)
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
            return user is null
                ? Array.Empty<string>()
                : user.FindAll("perm").Select(claim => claim.Value).ToArray();
        }
    }
}
