using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TailAdmin.Templates.Rcl.Options;
using TailAdmin.Templates.Rcl.Security;
using TailAdmin.Templates.Rcl.Services;

namespace TailAdmin.Templates.Rcl.ViewComponents;

/// <summary>侧边栏菜单视图组件。</summary>
public sealed class AdminMenuViewComponent : ViewComponent
{
    private readonly AdminMenuService _menuService;
    private readonly ICurrentAdmin _currentAdmin;

    /// <summary>初始化菜单视图组件。</summary>
    /// <param name="menuService">菜单服务。</param>
    /// <param name="currentAdmin">当前用户。</param>
    public AdminMenuViewComponent(AdminMenuService menuService, ICurrentAdmin currentAdmin)
    {
        _menuService = menuService;
        _currentAdmin = currentAdmin;
    }

    /// <summary>渲染菜单。</summary>
    /// <returns>菜单视图。</returns>
    public IViewComponentResult Invoke()
    {
        var viewModel = new AdminMenuViewModel
        {
            Items = _menuService.GetVisibleMenu(_currentAdmin),
            ActiveKey = ViewData["MenuKey"] as string,
            CurrentArea = ViewContext.RouteData.Values["area"]?.ToString(),
            CurrentController = ViewContext.RouteData.Values["controller"]?.ToString(),
            CurrentAction = ViewContext.RouteData.Values["action"]?.ToString()
        };

        return View(viewModel);
    }
}

/// <summary>侧边栏菜单视图模型。</summary>
public sealed class AdminMenuViewModel
{
    /// <summary>可见菜单项。</summary>
    public IReadOnlyList<AdminMenuItem> Items { get; set; } = Array.Empty<AdminMenuItem>();

    /// <summary>当前菜单键。</summary>
    public string? ActiveKey { get; set; }

    /// <summary>当前区域。</summary>
    public string? CurrentArea { get; set; }

    /// <summary>当前控制器。</summary>
    public string? CurrentController { get; set; }

    /// <summary>当前动作。</summary>
    public string? CurrentAction { get; set; }
}
