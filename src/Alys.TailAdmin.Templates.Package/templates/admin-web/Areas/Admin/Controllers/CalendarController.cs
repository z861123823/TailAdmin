using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>日历示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class CalendarController : Controller
{
    /// <summary>月视图日历。</summary>
    /// <param name="year">年份，默认当前年份。</param>
    /// <param name="month">月份，默认当前月份。</param>
    /// <returns>日历视图。</returns>
    [HttpGet]
    public IActionResult Index(int? year, int? month)
    {
        var today = DateTime.Today;
        var y = year ?? today.Year;
        var m = month ?? today.Month;
        if (m < 1)
        {
            m = 12;
            y--;
        }
        else if (m > 12)
        {
            m = 1;
            y++;
        }

        ViewData["Title"] = "日曆";
        ViewData["Subtitle"] = "Calendar：月視圖與日程安排";
        ViewData["MenuKey"] = "calendar";
        ViewData["Breadcrumb"] = new[] { "後台", "日曆" };
        ViewData["Year"] = y;
        ViewData["Month"] = m;
        return View();
    }
}
