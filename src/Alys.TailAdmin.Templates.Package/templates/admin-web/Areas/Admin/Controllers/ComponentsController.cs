using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>组件示例页（--sample false 时不生成）。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class ComponentsController : Controller
{
    /// <summary>组件总览页。</summary>
    /// <returns>组件示例视图。</returns>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "組件示例";
        ViewData["Subtitle"] = "TailAdmin 官方組件的 Razor 封裝";
        ViewData["MenuKey"] = "components";
        ViewData["Breadcrumb"] = new[] { "後台", "組件示例" };
        return View();
    }
}
