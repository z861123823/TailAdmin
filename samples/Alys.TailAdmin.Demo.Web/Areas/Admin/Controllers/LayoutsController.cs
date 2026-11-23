using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>布局变体示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class LayoutsController : Controller
{
    /// <summary>默认布局。</summary>
    /// <returns>布局视图。</returns>
    [HttpGet]
    public IActionResult Default() => Render("Default", "預設佈局", "layout-default", "側邊欄展開、頂欄固定、內容居中");

    /// <summary>侧栏收起。</summary>
    /// <returns>布局视图。</returns>
    [HttpGet]
    public IActionResult Collapsed()
    {
        ViewData["AdminSidebarCollapsed"] = true;
        return Render("Collapsed", "側欄收合", "layout-collapsed", "進入頁面時側邊欄預設收合");
    }

    /// <summary>隐藏侧栏。</summary>
    /// <returns>布局视图。</returns>
    [HttpGet]
    public IActionResult HiddenSidebar()
    {
        ViewData["AdminHideSidebar"] = true;
        return Render("HiddenSidebar", "無側欄", "layout-hidden-sidebar", "隱藏側邊欄，內容佔滿整行");
    }

    /// <summary>隐藏顶栏。</summary>
    /// <returns>布局视图。</returns>
    [HttpGet]
    public IActionResult HiddenHeader()
    {
        ViewData["AdminHideHeader"] = true;
        return Render("HiddenHeader", "無頂欄", "layout-hidden-header", "隱藏頂欄，適合大屏展示或嵌入式頁面");
    }

    /// <summary>全宽内容。</summary>
    /// <returns>布局视图。</returns>
    [HttpGet]
    public IActionResult FullWidth()
    {
        ViewData["AdminContentWidth"] = "full";
        return Render("FullWidth", "全寬內容", "layout-full", "移除內容最大寬度限制");
    }

    /// <summary>窄内容。</summary>
    /// <returns>布局视图。</returns>
    [HttpGet]
    public IActionResult Narrow()
    {
        ViewData["AdminContentWidth"] = "narrow";
        return Render("Narrow", "窄內容", "layout-narrow", "內容限制在 4xl 寬度，適合設定表單");
    }

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "佈局", title };
        return View(viewName);
    }
}
