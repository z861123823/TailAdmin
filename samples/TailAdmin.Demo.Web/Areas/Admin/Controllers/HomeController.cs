using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailAdmin.Demo.Web.Services;

namespace TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>仪表盘。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class HomeController : Controller
{
    private readonly DemoDataStore _store;

    /// <summary>初始化仪表盘控制器。</summary>
    /// <param name="store">演示数据仓储。</param>
    public HomeController(DemoDataStore store)
    {
        _store = store;
    }

    /// <summary>仪表盘首页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "儀表盤";
        ViewData["Subtitle"] = "商品與分類的即時概覽（演示數據）";
        ViewData["MenuKey"] = "dashboard";
        ViewData["Breadcrumb"] = new[] { "後台", "儀表盤" };
        return View(_store.GetDashboard());
    }
}
