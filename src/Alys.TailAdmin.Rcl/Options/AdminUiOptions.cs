namespace Alys.TailAdmin.Rcl.Options;

/// <summary>
/// 后台界面配置，对应 appsettings.json 的 AdminUi 节。
/// </summary>
public sealed class AdminUiOptions
{
    /// <summary>配置节名称。</summary>
    public const string SectionName = "AdminUi";

    /// <summary>站点名称，用于侧边栏与浏览器标题前缀。</summary>
    public string SiteName { get; set; } = "管理后台";

    /// <summary>Logo 地址；为空时使用主题色首字母占位。</summary>
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>品牌主题名，必须与 frontend/theme/brand.json 的 name 一致。</summary>
    public string BrandName { get; set; } = "als";

    /// <summary>默认主题：light / dark / system。</summary>
    public string DefaultTheme { get; set; } = "system";

    /// <summary>桌面端侧边栏默认是否折叠。</summary>
    public bool SidebarCollapsed { get; set; }

    /// <summary>顶栏是否显示全局搜索入口。</summary>
    public bool EnableSearch { get; set; }

    /// <summary>顶栏全局搜索提交地址；为空时搜索框不显示。</summary>
    public string SearchUrl { get; set; } = string.Empty;

    /// <summary>登录页地址，供 401 跳转使用。</summary>
    public string LoginPath { get; set; } = "/Admin/Account/Login";

    /// <summary>登出地址，顶栏用户菜单以 POST 方式提交。</summary>
    public string LogoutPath { get; set; } = "/Admin/Account/Logout";

    /// <summary>菜单树。</summary>
    public AdminMenuOptions Menu { get; set; } = new();
}
