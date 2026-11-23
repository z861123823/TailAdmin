using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>通用页面示例：空白页、404、即將上線與系統維護。</summary>
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public sealed class PagesController : Controller
{
    /// <summary>空白页。</summary>
    /// <returns>空白页视图。</returns>
    [Authorize]
    [HttpGet]
    public IActionResult Blank()
    {
        ViewData["Title"] = "空白頁";
        ViewData["Subtitle"] = "用於承載自定義內容的空白頁面";
        ViewData["MenuKey"] = "page-blank";
        ViewData["Breadcrumb"] = new[] { "後台", "頁面", "空白頁" };
        return View();
    }

    /// <summary>404 页面。</summary>
    /// <returns>404 视图。</returns>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Error404()
    {
        ViewData["Title"] = "404";
        return View();
    }

    /// <summary>即将上线页。</summary>
    /// <returns>即将上线视图。</returns>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult ComingSoon()
    {
        ViewData["Title"] = "即將上線";
        return View();
    }

    /// <summary>系统维护页。</summary>
    /// <returns>系统维护视图。</returns>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Maintenance()
    {
        ViewData["Title"] = "系統維護";
        return View();
    }
}
