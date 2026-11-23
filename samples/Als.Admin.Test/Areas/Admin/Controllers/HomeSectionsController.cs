using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>首页展示方式设置。</summary>
[Area("Admin")]
public sealed class HomeSectionsController : AlsControllerBase
{
    private readonly AlsAdminGateway _gateway;

    /// <summary>构造函数。</summary>
    public HomeSectionsController(AlsApiClient api, AlsAdminGateway gateway, ILogger<HomeSectionsController> logger)
        : base(api, logger)
    {
        _gateway = gateway;
    }

    /// <summary>首页设置页。</summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "首頁設置";
        ViewData["Subtitle"] = "切換首頁為推薦 / 熱門 / 精選三段，或按商品分類分段展示";
        ViewData["MenuKey"] = "home-sections";
        ViewData["Breadcrumb"] = new[] { "後台", "內容中心", "首頁設置" };

        var config = await _gateway.GetHomeSectionsAsync(cancellationToken);
        var model = new HomeSectionsPageModel
        {
            Mode = config.Mode,
            CategoryIds = config.CategoryIds.ToList(),
            Types = await _gateway.GetGoodsTypesAsync(cancellationToken)
        };

        return View(model);
    }

    /// <summary>保存首页展示方式。</summary>
    [HttpPost]
    public Task<IActionResult> Save(string? mode, int[]? categoryIds, CancellationToken cancellationToken)
    {
        var fields = new List<KeyValuePair<string, string?>>
        {
            new("mode", string.IsNullOrWhiteSpace(mode) ? "flags" : mode)
        };

        foreach (var id in categoryIds ?? Array.Empty<int>())
        {
            fields.Add(new KeyValuePair<string, string?>("categoryIds", id.ToString()));
        }

        return ForwardPostAsync("/Admin/HomeSections/Save", fields, cancellationToken);
    }
}
