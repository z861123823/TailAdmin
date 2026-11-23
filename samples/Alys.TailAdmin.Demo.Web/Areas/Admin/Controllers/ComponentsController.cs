using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>旧组件总览入口，保留地址并跳转到逐组件页面。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class ComponentsController : Controller
{
    /// <summary>跳转到 UI 元件首个页面。</summary>
    /// <returns>重定向结果。</returns>
    [HttpGet]
    public IActionResult Index() => RedirectToAction("Alerts", "UiElements", new { area = "Admin" });
}
