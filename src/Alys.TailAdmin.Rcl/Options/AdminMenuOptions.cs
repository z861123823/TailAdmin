namespace Alys.TailAdmin.Rcl.Options;

/// <summary>菜单配置根节点。</summary>
public sealed class AdminMenuOptions
{
    /// <summary>一级菜单集合。</summary>
    public List<AdminMenuItem> Items { get; set; } = new();
}

/// <summary>菜单项。可由 Controller/Action/Area 生成路由，也可直接配置 Url。</summary>
public sealed class AdminMenuItem
{
    /// <summary>菜单唯一键，页面通过 ViewData["MenuKey"] 指定当前高亮项。</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>菜单标题。</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>图标名，见 RCL 的 _Icon.cshtml 支持列表。</summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>区域名。</summary>
    public string? Area { get; set; }

    /// <summary>控制器名。</summary>
    public string? Controller { get; set; }

    /// <summary>动作名。</summary>
    public string? Action { get; set; }

    /// <summary>直接链接；与 Controller/Action 二选一。</summary>
    public string? Url { get; set; }

    /// <summary>权限码；为空表示所有已登录用户可见。</summary>
    public string? Permission { get; set; }

    /// <summary>是否新窗口打开。</summary>
    public bool NewWindow { get; set; }

    /// <summary>子菜单。</summary>
    public List<AdminMenuItem> Children { get; set; } = new();
}
