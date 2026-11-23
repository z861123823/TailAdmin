using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>仪表盘。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class HomeController : Controller
{
    /// <summary>仪表盘首页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "儀表盤";
        ViewData["Subtitle"] = "Alys.TailAdmin 通用後台模板已就緒";
        ViewData["MenuKey"] = "dashboard";
        ViewData["Breadcrumb"] = new[] { "後台", "儀表盤" };
        return View();
    }
}
