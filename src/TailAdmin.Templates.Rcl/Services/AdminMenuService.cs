using Microsoft.Extensions.Options;
using TailAdmin.Templates.Rcl.Options;
using TailAdmin.Templates.Rcl.Security;

namespace TailAdmin.Templates.Rcl.Services;

/// <summary>
/// 菜单服务：按权限过滤菜单树，并提供菜单键存在性校验。
/// </summary>
public sealed class AdminMenuService
{
    private readonly AdminUiOptions _options;

    /// <summary>初始化菜单服务。</summary>
    /// <param name="options">后台界面配置。</param>
    public AdminMenuService(IOptions<AdminUiOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
    }

    /// <summary>
    /// 获取当前用户可见的菜单树。无权限的菜单不会渲染，而不是渲染后点击报 403。
    /// </summary>
    /// <param name="admin">当前用户。</param>
    /// <returns>过滤后的菜单集合。</returns>
    public IReadOnlyList<AdminMenuItem> GetVisibleMenu(ICurrentAdmin admin)
    {
        ArgumentNullException.ThrowIfNull(admin);
        return Filter(_options.Menu.Items, admin);
    }

    /// <summary>
    /// 判断菜单键是否存在于配置中（不区分权限）。
    /// </summary>
    /// <param name="key">菜单键。</param>
    /// <returns>是否存在。</returns>
    public bool ContainsKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return Find( _options.Menu.Items, key) is not null;
    }

    private static AdminMenuItem? Find(IEnumerable<AdminMenuItem> items, string key)
    {
        foreach (var item in items)
        {
            if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }

            var child = Find(item.Children, key);
            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private static List<AdminMenuItem> Filter(IEnumerable<AdminMenuItem> items, ICurrentAdmin admin)
    {
        var result = new List<AdminMenuItem>();
        foreach (var item in items)
        {
            if (!admin.HasPermission(item.Permission))
            {
                continue;
            }

            var children = Filter(item.Children, admin);
            var hasLocation = !string.IsNullOrWhiteSpace(item.Url)
                || !string.IsNullOrWhiteSpace(item.Controller)
                || !string.IsNullOrWhiteSpace(item.Action);

            if (!hasLocation && children.Count == 0)
            {
                continue;
            }

            result.Add(new AdminMenuItem
            {
                Key = item.Key,
                Title = item.Title,
                Icon = item.Icon,
                Area = item.Area,
                Controller = item.Controller,
                Action = item.Action,
                Url = item.Url,
                Permission = item.Permission,
                NewWindow = item.NewWindow,
                Children = children
            });
        }

        return result;
    }
}
