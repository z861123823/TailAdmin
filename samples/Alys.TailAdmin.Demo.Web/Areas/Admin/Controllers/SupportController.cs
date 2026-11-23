using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>客服工单示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class SupportController : Controller
{
    /// <summary>工单列表。</summary>
    /// <returns>工单列表视图。</returns>
    [HttpGet]
    public IActionResult Tickets()
    {
        ViewData["Title"] = "支援工單";
        ViewData["Subtitle"] = "Support：工單狀態、優先級與負責人";
        ViewData["MenuKey"] = "support";
        ViewData["Breadcrumb"] = new[] { "後台", "客服", "支援工單" };
        return View();
    }
}
