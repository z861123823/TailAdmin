using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>表单示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class FormsController : Controller
{
    /// <summary>表单元素。</summary>
    /// <returns>表单元素视图。</returns>
    [HttpGet]
    public IActionResult Elements() => Render("Elements", "表單元素", "form-elements", "Form Elements：輸入框、下拉、文字域與開關");

    /// <summary>表单布局。</summary>
    /// <returns>表单布局视图。</returns>
    [HttpGet]
    public IActionResult Layout() => Render("Layout", "表單佈局", "form-layout", "Form Layout：分區、雙欄與操作欄");

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "表單", title };
        return View(viewName);
    }
}
