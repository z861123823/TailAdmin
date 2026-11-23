using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>仪表盘变体示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class DashboardsController : Controller
{
    /// <summary>分析仪表盘。</summary>
    /// <returns>分析视图。</returns>
    [HttpGet]
    public IActionResult Analytics() => Render("Analytics", "分析", "dash-analytics", "Analytics：流量、轉化與熱門頁面");

    /// <summary>营销仪表盘。</summary>
    /// <returns>营销视图。</returns>
    [HttpGet]
    public IActionResult Marketing() => Render("Marketing", "營銷", "dash-marketing", "Marketing：投放、ROI 與活動表現");

    /// <summary>CRM 仪表盘。</summary>
    /// <returns>CRM 视图。</returns>
    [HttpGet]
    public IActionResult Crm() => Render("Crm", "CRM", "dash-crm", "CRM：線索、商機與成交");

    /// <summary>SaaS 仪表盘。</summary>
    /// <returns>SaaS 视图。</returns>
    [HttpGet]
    public IActionResult Saas() => Render("Saas", "SaaS", "dash-saas", "SaaS：MRR、流失與訂閱");

    /// <summary>物流仪表盘。</summary>
    /// <returns>物流视图。</returns>
    [HttpGet]
    public IActionResult Logistics() => Render("Logistics", "物流", "dash-logistics", "Logistics：發貨、運輸與異常");

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "儀表盤", title };
        return View(viewName);
    }
}
