using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>图表示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class ChartsController : Controller
{
    /// <summary>折线图。</summary>
    /// <returns>折线图视图。</returns>
    [HttpGet]
    public IActionResult Line() => Render("Line", "折線圖", "chart-line", "Line Chart：平滑曲線與漸變填充");

    /// <summary>柱状图。</summary>
    /// <returns>柱状图视图。</returns>
    [HttpGet]
    public IActionResult Bar() => Render("Bar", "長條圖", "chart-bar", "Bar Chart：分組對比與圓角柱體");

    /// <summary>饼图。</summary>
    /// <returns>饼图视图。</returns>
    [HttpGet]
    public IActionResult Pie() => Render("Pie", "圓餅圖", "chart-pie", "Pie / Donut：佔比與中心合計");

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "圖表", title };
        return View(viewName);
    }
}
