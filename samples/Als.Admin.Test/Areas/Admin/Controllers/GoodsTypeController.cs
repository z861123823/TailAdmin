using System.Globalization;
using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>商品分类管理。</summary>
[Area("Admin")]
public sealed class GoodsTypeController : AlsControllerBase
{
    private readonly AlsAdminGateway _gateway;

    /// <summary>构造函数。</summary>
    public GoodsTypeController(AlsApiClient api, AlsAdminGateway gateway, ILogger<GoodsTypeController> logger)
        : base(api, logger)
    {
        _gateway = gateway;
    }

    /// <summary>分类列表页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "商品分類";
        ViewData["Subtitle"] = "分類新增、修改與刪除，刪除前商城會檢查是否有商品或子分類佔用";
        ViewData["MenuKey"] = "goods-type";
        ViewData["Breadcrumb"] = new[] { "後台", "商品中心", "商品分類" };
        return View();
    }

    /// <summary>分类列表。</summary>
    [HttpGet]
    public Task<IActionResult> List(CancellationToken cancellationToken)
        => ForwardListAsync("/Admin/GoodsType/List", new Dictionary<string, string?>(), cancellationToken);

    /// <summary>分类编辑弹窗。</summary>
    [HttpGet]
    public async Task<IActionResult> EditForm(int typeId, string? typeName, int parentId, CancellationToken cancellationToken)
    {
        var model = new GoodsTypeFormModel
        {
            TypeId = typeId,
            TypeName = typeName,
            ParentId = parentId,
            Types = await _gateway.GetGoodsTypesAsync(cancellationToken)
        };
        return PartialView("_Form", model);
    }

    /// <summary>保存分类。</summary>
    [HttpPost]
    public Task<IActionResult> Save(int typeId, string? typeName, int parentId, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/GoodsType/Save", new[]
        {
            Pair("typeId", typeId),
            Pair("typeName", typeName),
            Pair("parentId", parentId)
        }, cancellationToken);

    /// <summary>删除分类。</summary>
    [HttpPost]
    public Task<IActionResult> Delete(int typeId, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/GoodsType/Delete", new[] { Pair("typeId", typeId) }, cancellationToken);

    private static KeyValuePair<string, string?> Pair(string key, object? value)
        => new(key, value is null ? null : Convert.ToString(value, CultureInfo.InvariantCulture));
}
