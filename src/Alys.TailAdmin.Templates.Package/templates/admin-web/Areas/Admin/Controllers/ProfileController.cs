using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>个人资料示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class ProfileController : Controller
{
    /// <summary>个人资料页。</summary>
    /// <returns>个人资料视图。</returns>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "個人資料";
        ViewData["Subtitle"] = "User Profile：資料、設定與通知";
        ViewData["MenuKey"] = "profile";
        ViewData["Breadcrumb"] = new[] { "後台", "個人資料" };
        return View();
    }
}
