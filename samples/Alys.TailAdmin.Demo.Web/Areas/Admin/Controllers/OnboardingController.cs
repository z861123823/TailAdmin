using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>引导流程示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class OnboardingController : Controller
{
    /// <summary>三步引导向导。</summary>
    /// <returns>引导流程视图。</returns>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "引導流程";
        ViewData["Subtitle"] = "Onboarding：三步引導與進度";
        ViewData["MenuKey"] = "onboarding";
        ViewData["Breadcrumb"] = new[] { "後台", "引導流程" };
        return View();
    }
}
