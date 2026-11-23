using System.Globalization;
using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>商品管理：列表、弹窗编辑、保存、删除、标记开关与图片上传。</summary>
[Area("Admin")]
public sealed class GoodsController : AlsControllerBase
{
    private readonly AlsAdminGateway _gateway;

    /// <summary>构造函数。</summary>
    public GoodsController(AlsApiClient api, AlsAdminGateway gateway, ILogger<GoodsController> logger)
        : base(api, logger)
    {
        _gateway = gateway;
    }

    /// <summary>商品列表页。</summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "商品管理";
        ViewData["Subtitle"] = "對接商城 /Admin/Goods 接口，支援分類篩選、編輯與上下架標記";
        ViewData["MenuKey"] = "goods";
        ViewData["Breadcrumb"] = new[] { "後台", "商品中心", "商品管理" };

        var model = new GoodsListPageModel { Types = await _gateway.GetGoodsTypesAsync(cancellationToken) };
        return View(model);
    }

    /// <summary>商品列表数据源（layui 格式）。</summary>
    [HttpGet]
    public Task<IActionResult> List(int page = 1, int limit = 20, int typeId = 0, CancellationToken cancellationToken = default)
        => ForwardListAsync("/Admin/Goods/List", new Dictionary<string, string?>
        {
            ["page"] = Math.Max(1, page).ToString(CultureInfo.InvariantCulture),
            ["limit"] = Math.Clamp(limit, 1, 200).ToString(CultureInfo.InvariantCulture),
            ["typeId"] = Math.Max(0, typeId).ToString(CultureInfo.InvariantCulture)
        }, cancellationToken);

    /// <summary>商品编辑弹窗。</summary>
    [HttpGet]
    public async Task<IActionResult> EditForm(int id, CancellationToken cancellationToken)
    {
        var types = await _gateway.GetGoodsTypesAsync(cancellationToken);
        if (id <= 0)
        {
            return PartialView("_Form", new GoodsFormModel
            {
                Types = types,
                UploadUrl = Url.Action("UploadImage", "Goods", new { area = "Admin" }) ?? string.Empty
            });
        }

        var node = await Api.GetJsonAsync("/Admin/Goods/Get", new Dictionary<string, string?>
        {
            ["id"] = id.ToString(CultureInfo.InvariantCulture)
        }, cancellationToken);
        var goods = AlsJson.Object(node, "Goods");
        if (goods is null)
        {
            return Content("<div class=\"p-4 text-theme-sm text-red-500\">商城未返回該商品，請重新整理後再試。</div>", "text/html; charset=utf-8");
        }

        var model = new GoodsFormModel
        {
            Id = id,
            GoodsNumber = AlsJson.Str(goods, "GoodsNumber"),
            GoodsName = AlsJson.Str(goods, "GoodsName"),
            Price = AlsJson.Decimal(goods, "Price"),
            TypeId = AlsJson.Int(goods, "TypeId"),
            Format = AlsJson.Str(goods, "Format"),
            ProductDescription = AlsJson.Str(goods, "ProductDescription"),
            SpecData = NullIfBlank(AlsJson.Str(goods, "SpecData"), "[]"),
            SkuData = NullIfBlank(AlsJson.Str(goods, "SKUData"), "{}"),
            SkuTableList = NullIfBlank(AlsJson.Str(goods, "SKUtableList"), "[]"),
            ExtraImagesJson = await _gateway.GetExtraImagesJsonAsync(id, cancellationToken),
            PicName = AlsJson.Str(goods, "PicName"),
            PicUrl = AlsJson.AssetUrl(AlsJson.Str(goods, "PicUrls")),
            IsHot = AlsJson.Int(goods, "IsHot") == 1,
            IsRecommend = AlsJson.Int(goods, "IsRecommend") == 1,
            IsSift = AlsJson.Int(goods, "IsSift") == 1,
            Types = types,
            UploadUrl = Url.Action("UploadImage", "Goods", new { area = "Admin" }) ?? string.Empty
        };

        return PartialView("_Form", model);
    }

    /// <summary>保存商品。</summary>
    [HttpPost]
    public Task<IActionResult> Save(
        int id,
        string? goodsNumber,
        string? goodsName,
        decimal price,
        int typeId,
        string? format,
        string? goodsImg,
        string? productDescription,
        string? specData,
        string? skuData,
        string? skuTableList,
        string? extraImagesJson,
        int isHot,
        int isRecommend,
        int isSift,
        CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Goods/Save", new[]
        {
            Pair("id", id),
            Pair("goodsNumber", goodsNumber),
            Pair("goodsName", goodsName),
            Pair("price", price.ToString(CultureInfo.InvariantCulture)),
            Pair("typeId", typeId),
            Pair("format", format),
            Pair("goodsImg", goodsImg),
            Pair("productDescription", productDescription),
            Pair("specData", specData),
            Pair("skuData", skuData),
            Pair("skuTableList", skuTableList),
            Pair("extraImagesJson", extraImagesJson),
            Pair("isHot", isHot),
            Pair("isRecommend", isRecommend),
            Pair("isSift", isSift)
        }, cancellationToken);

    /// <summary>删除商品。</summary>
    [HttpPost]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Goods/Delete", new[] { Pair("id", id) }, cancellationToken);

    /// <summary>修改热销 / 推荐 / 精选标记。</summary>
    [HttpPost]
    public Task<IActionResult> SetFlag(int id, string? field, int value, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Goods/SetFlag", new[]
        {
            Pair("id", id),
            Pair("field", field),
            Pair("value", value)
        }, cancellationToken);

    /// <summary>上传商品图片。</summary>
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return Fail("請選擇要上傳的圖片");
        }

        using var content = AlsApiClient.CreateFileContent(file);
        var body = await Api.PostMultipartAsync("/Admin/GoodsImage/UploadImage", content, cancellationToken);

        try
        {
            var node = System.Text.Json.Nodes.JsonNode.Parse(body);
            if (node is not null)
            {
                return JsonNodeResult(node);
            }
        }
        catch (System.Text.Json.JsonException)
        {
            // 非 JSON 响应按原文返回，便于定位
        }

        return Content(body, "application/json; charset=utf-8");
    }

    private static string NullIfBlank(string? value, string fallback)
        => string.IsNullOrWhiteSpace(value) ? fallback : value;

    private static KeyValuePair<string, string?> Pair(string key, object? value)
        => new(key, value?.ToString());
}
