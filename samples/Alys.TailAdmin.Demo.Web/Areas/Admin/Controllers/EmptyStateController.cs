using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>空状态示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class EmptyStateController : Controller
{
    /// <summary>空状态总览。</summary>
    /// <returns>空状态视图。</returns>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "空狀態";
        ViewData["Subtitle"] = "Empty State：無數據、無結果、無通知與加載失敗";
        ViewData["MenuKey"] = "empty-state";
        ViewData["Breadcrumb"] = new[] { "後台", "空狀態" };
        return View();
    }
}
