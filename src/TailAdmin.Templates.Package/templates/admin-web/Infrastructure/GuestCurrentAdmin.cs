using TailAdmin.Templates.Rcl.Security;

namespace TailAdminHost.Infrastructure;

/// <summary>未启用鉴权时的当前用户实现：菜单全部可见，不显示登出入口。</summary>
public sealed class GuestCurrentAdmin : ICurrentAdmin
{
    /// <inheritdoc />
    public bool IsAuthenticated => false;

    /// <inheritdoc />
    public string? UserName => "guest";

    /// <inheritdoc />
    public string? DisplayName => "訪客";

    /// <inheritdoc />
    public IReadOnlyCollection<string> Permissions { get; } = new[] { "*" };
}
