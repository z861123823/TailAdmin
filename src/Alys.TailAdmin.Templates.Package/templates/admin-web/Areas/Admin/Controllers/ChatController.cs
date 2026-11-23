using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>聊天示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class ChatController : Controller
{
    /// <summary>聊天面板。</summary>
    /// <returns>聊天视图。</returns>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "聊天";
        ViewData["Subtitle"] = "Chat：聯絡人列表與對話";
        ViewData["MenuKey"] = "chat";
        ViewData["Breadcrumb"] = new[] { "後台", "聊天" };
        return View();
    }
}
