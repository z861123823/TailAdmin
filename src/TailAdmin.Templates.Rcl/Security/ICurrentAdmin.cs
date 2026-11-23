namespace TailAdmin.Templates.Rcl.Security;

/// <summary>
/// 当前后台用户抽象。用户体系、登录与角色由宿主项目实现，模板只消费该接口。
/// </summary>
public interface ICurrentAdmin
{
    /// <summary>是否已登录。</summary>
    bool IsAuthenticated { get; }

    /// <summary>登录账号。</summary>
    string? UserName { get; }

    /// <summary>显示名称。</summary>
    string? DisplayName { get; }

    /// <summary>权限码集合；包含 "*" 表示拥有全部权限。</summary>
    IReadOnlyCollection<string> Permissions { get; }
}

/// <summary>
/// 当前用户扩展方法。
/// </summary>
public static class CurrentAdminExtensions
{
    /// <summary>
    /// 判断当前用户是否拥有指定权限；权限码为空时视为无需权限。
    /// </summary>
    /// <param name="admin">当前用户。</param>
    /// <param name="permission">权限码。</param>
    /// <returns>是否拥有权限。</returns>
    public static bool HasPermission(this ICurrentAdmin admin, string? permission)
    {
        ArgumentNullException.ThrowIfNull(admin);
        if (string.IsNullOrWhiteSpace(permission))
        {
            return true;
        }

        return admin.Permissions.Contains("*", StringComparer.OrdinalIgnoreCase)
            || admin.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// 默认实现：未接入鉴权的宿主使用，放行全部权限。
/// </summary>
public sealed class AllowAllCurrentAdmin : ICurrentAdmin
{
    /// <inheritdoc />
    public bool IsAuthenticated => true;

    /// <inheritdoc />
    public string? UserName => "admin";

    /// <inheritdoc />
    public string? DisplayName => "管理員";

    /// <inheritdoc />
    public IReadOnlyCollection<string> Permissions { get; } = new[] { "*" };
}
