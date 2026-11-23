using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>表格示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class TablesController : Controller
{
    /// <summary>基础表格。</summary>
    /// <returns>基础表格视图。</returns>
    [HttpGet]
    public IActionResult Basic() => Render("Basic", "基礎表格", "table-basic", "Basic Tables：靜態表頭、行狀態與操作");

    /// <summary>数据表格。</summary>
    /// <returns>数据表格视图。</returns>
    [HttpGet]
    public IActionResult Data() => Render("Data", "數據表格", "table-data", "Data Tables：分頁、排序、查詢與固定列");

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "表格", title };
        return View(viewName);
    }
}
