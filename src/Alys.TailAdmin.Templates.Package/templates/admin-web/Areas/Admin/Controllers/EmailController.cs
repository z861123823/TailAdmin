using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>邮件示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class EmailController : Controller
{
    /// <summary>收件箱。</summary>
    /// <returns>收件箱视图。</returns>
    [HttpGet]
    public IActionResult Inbox()
    {
        ViewData["Title"] = "收件箱";
        ViewData["Subtitle"] = "Email：郵件列表與閱讀面板";
        ViewData["MenuKey"] = "email";
        ViewData["Breadcrumb"] = new[] { "後台", "郵件", "收件箱" };
        return View();
    }
}
