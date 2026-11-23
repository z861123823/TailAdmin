using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>示例列表页：演示服务端分页、排序与关键字查询（--sample false 时不生成）。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class GoodsController : Controller
{
    private static readonly List<GoodsRow> Seed = new()
    {
        new(1, "無線藍牙耳機", 299.00m, "數碼配件", 128, "2026-11-20 09:00:00"),
        new(2, "機械鍵盤", 459.00m, "數碼配件", 96, "2026-11-19 10:30:00"),
        new(3, "人體工學椅", 1299.00m, "家居生活", 42, "2026-11-18 14:10:00"),
        new(4, "保溫杯", 89.00m, "家居生活", 210, "2026-11-17 08:45:00"),
        new(5, "瑜伽墊", 129.00m, "運動戶外", 175, "2026-11-16 16:20:00"),
        new(6, "香薰蠟燭", 69.00m, "家居生活", 88, "2026-11-15 11:05:00"),
        new(7, "咖啡豆禮盒", 168.00m, "食品飲料", 134, "2026-11-14 19:40:00"),
        new(8, "露營帳篷", 899.00m, "運動戶外", 31, "2026-11-13 12:00:00"),
        new(9, "智能手環", 399.00m, "數碼配件", 156, "2026-11-12 13:15:00"),
        new(10, "護膚套裝", 520.00m, "美妝個護", 64, "2026-11-11 10:00:00"),
        new(11, "便攜充電寶", 159.00m, "數碼配件", 240, "2026-11-10 17:30:00"),
        new(12, "桌面收納盒", 49.00m, "家居生活", 190, "2026-11-09 09:50:00")
    };

    /// <summary>列表页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "商品管理";
        ViewData["Subtitle"] = "示例頁：可直接在此基礎上接入真實介面";
        ViewData["MenuKey"] = "goods";
        ViewData["Breadcrumb"] = new[] { "後台", "商品中心", "商品管理" };
        return View();
    }

    /// <summary>列表接口：{ code, msg, count, data }。</summary>
    [HttpGet]
    public IActionResult List(int page = 1, int limit = 20, string? sortField = null, string? sortOrder = "desc", string? keyword = null)
    {
        page = Math.Max(1, page);
        limit = Math.Clamp(limit, 1, 200);

        IEnumerable<GoodsRow> query = Seed;
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(item => item.GoodsName.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        query = (sortField, sortOrder?.ToLowerInvariant()) switch
        {
            ("GoodsName", "asc") => query.OrderBy(item => item.GoodsName, StringComparer.Ordinal),
            ("GoodsName", _) => query.OrderByDescending(item => item.GoodsName, StringComparer.Ordinal),
            ("Price", "asc") => query.OrderBy(item => item.Price),
            ("Price", _) => query.OrderByDescending(item => item.Price),
            ("SalesVolume", "asc") => query.OrderBy(item => item.SalesVolume),
            ("SalesVolume", _) => query.OrderByDescending(item => item.SalesVolume),
            _ => query.OrderBy(item => item.Id)
        };

        var total = query.Count();
        var rows = query.Skip((page - 1) * limit).Take(limit).ToList();
        return Json(new Dictionary<string, object?>
        {
            ["code"] = 0,
            ["msg"] = string.Empty,
            ["count"] = total,
            ["data"] = rows
        });
    }

    /// <summary>列表行。</summary>
    public sealed record GoodsRow(int Id, string GoodsName, decimal Price, string TypeName, int SalesVolume, string AddTime);
}
