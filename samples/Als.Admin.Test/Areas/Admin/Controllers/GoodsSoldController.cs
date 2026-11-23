using System.Globalization;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>销售统计（订单商品明细）：查看与清理。</summary>
[Area("Admin")]
public sealed class GoodsSoldController : AlsControllerBase
{
    /// <summary>构造函数。</summary>
    public GoodsSoldController(AlsApiClient api, ILogger<GoodsSoldController> logger) : base(api, logger)
    {
    }

    /// <summary>销售统计页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "銷售統計";
        ViewData["Subtitle"] = "對接商城商品明細，可查看每筆銷售並清理異常記錄";
        ViewData["MenuKey"] = "goods-sold";
        ViewData["Breadcrumb"] = new[] { "後台", "交易中心", "銷售統計" };
        return View();
    }

    /// <summary>销售记录数据源。</summary>
    [HttpGet]
    public Task<IActionResult> List(int page = 1, int limit = 15, CancellationToken cancellationToken = default)
        => ForwardListAsync("/Admin/GoodsSold/List", new Dictionary<string, string?>
        {
            ["page"] = Math.Max(1, page).ToString(CultureInfo.InvariantCulture),
            ["limit"] = Math.Clamp(limit, 1, 200).ToString(CultureInfo.InvariantCulture)
        }, cancellationToken);

    /// <summary>删除一条销售记录。</summary>
    [HttpPost]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/GoodsSold/Delete", new[]
        {
            new KeyValuePair<string, string?>("id", id.ToString(CultureInfo.InvariantCulture))
        }, cancellationToken);
}
