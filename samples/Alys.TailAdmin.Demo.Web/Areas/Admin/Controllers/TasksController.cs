using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>任务看板示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class TasksController : Controller
{
    /// <summary>任务看板。</summary>
    /// <returns>看板视图。</returns>
    [HttpGet]
    public IActionResult Kanban()
    {
        ViewData["Title"] = "任務看板";
        ViewData["Subtitle"] = "Tasks：待處理、進行中與已完成";
        ViewData["MenuKey"] = "tasks";
        ViewData["Breadcrumb"] = new[] { "後台", "任務", "看板" };
        return View();
    }
}
