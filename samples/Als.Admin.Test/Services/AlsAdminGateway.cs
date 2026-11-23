using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Als.Admin.Test.Services;

/// <summary>分类下拉选项。</summary>
public sealed record GoodsTypeOption(int TypeId, string TypeName, int ParentId);

/// <summary>文章分类下拉选项。</summary>
public sealed record ArticleTypeOption(int Id, string TypeName);

/// <summary>文章编辑数据。</summary>
public sealed record ArticleEditData(string Title, int TypeId, string Contents);

/// <summary>首页展示方式配置。</summary>
public sealed record HomeSectionsData(string Mode, IReadOnlyList<int> CategoryIds);

/// <summary>
/// 商城后台接口的读取与解析聚合。商城后台的编辑页没有 JSON 详情接口，
/// 这里通过解析页面中的既有数据（富文本、多图、首页配置）补齐测试后台需要的表单数据。
/// </summary>
public sealed class AlsAdminGateway
{
    private static readonly Regex ExtraImagesRegex = new(
        "var\\s+extraImages\\s*=\\s*(\\[[\\s\\S]*?\\]);",
        RegexOptions.Compiled);

    private static readonly Regex TitleInputRegex = new(
        "<input[^>]*\\bid=\"Title\"[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ValueAttrRegex = new(
        "value=\"([^\"]*)\"",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ContentsRegex = new(
        "<textarea[^>]*\\bid=\"Contents\"[^>]*>([\\s\\S]*?)</textarea>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ArticleTypeSelectRegex = new(
        "<select[^>]*\\bid=\"TypeId\"[^>]*>([\\s\\S]*?)</select>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex SelectedOptionRegex = new(
        "<option[^>]*value=\"(\\d+)\"[^>]*selected",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex SelectedTypesBlockRegex = new(
        "<ol[^>]*\\bid=\"selectedTypes\"[^>]*>([\\s\\S]*?)</ol>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex TypeIdAttrRegex = new(
        "data-typeid=\"(\\d+)\"",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex FlagsModeRegex = new(
        "name=\"Mode\"\\s+value=\"flags\"[^>]*checked",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex CategoryModeRegex = new(
        "name=\"Mode\"\\s+value=\"category\"[^>]*checked",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly AlsApiClient _api;
    private readonly ILogger<AlsAdminGateway> _logger;

    /// <summary>构造函数。</summary>
    public AlsAdminGateway(AlsApiClient api, ILogger<AlsAdminGateway> logger)
    {
        _api = api;
        _logger = logger;
    }

    /// <summary>读取商品分类。</summary>
    public async Task<List<GoodsTypeOption>> GetGoodsTypesAsync(CancellationToken cancellationToken = default)
    {
        var node = await _api.GetJsonAsync("/Admin/GoodsType/List", cancellationToken: cancellationToken);
        var rows = AlsJson.Array(node, "data");
        if (rows is null)
        {
            return new List<GoodsTypeOption>();
        }

        return rows
            .Select(row => new GoodsTypeOption(
                AlsJson.Int(row, "TypeId"),
                AlsJson.Str(row, "TypeName") ?? string.Empty,
                AlsJson.Int(row, "ParentId")))
            .Where(item => item.TypeId > 0)
            .ToList();
    }

    /// <summary>读取文章分类。</summary>
    public async Task<List<ArticleTypeOption>> GetArticleTypesAsync(CancellationToken cancellationToken = default)
    {
        var node = await _api.GetJsonAsync("/Admin/ArticleType/List", cancellationToken: cancellationToken);
        var rows = AlsJson.Array(node, "data");
        if (rows is null)
        {
            return new List<ArticleTypeOption>();
        }

        return rows
            .Select(row => new ArticleTypeOption(
                AlsJson.Int(row, "Id"),
                AlsJson.Str(row, "TypeName") ?? string.Empty))
            .Where(item => item.Id > 0)
            .ToList();
    }

    /// <summary>
    /// 从商品编辑页读取额外多图，并统一转成商城保存接口要求的
    /// <c>[{ picId, sortOrder }]</c>（键名必须小写，商城按此严格解析）。
    /// </summary>
    public async Task<string> GetExtraImagesJsonAsync(int goodsId, CancellationToken cancellationToken = default)
    {
        if (goodsId <= 0)
        {
            return "[]";
        }

        try
        {
            var html = await _api.GetTextAsync($"/Admin/Goods/Edit/{goodsId}", cancellationToken: cancellationToken);
            var match = ExtraImagesRegex.Match(html);
            if (!match.Success)
            {
                return "[]";
            }

            var source = JsonNode.Parse(match.Groups[1].Value) as JsonArray;
            if (source is null)
            {
                return "[]";
            }

            var result = new JsonArray();
            foreach (var item in source)
            {
                var picId = AlsJson.Int(item, "picId");
                if (picId <= 0)
                {
                    continue;
                }

                result.Add(new JsonObject
                {
                    ["picId"] = picId,
                    ["sortOrder"] = AlsJson.Int(item, "sortOrder")
                });
            }

            return result.ToJsonString();
        }
        catch (Exception exception) when (exception is JsonException or HttpRequestException or InvalidOperationException)
        {
            _logger.LogWarning(exception, "读取商品 {GoodsId} 的额外图片失败，按无额外图片处理", goodsId);
            return "[]";
        }
    }

    /// <summary>解析文章编辑页中的标题、分类与正文。</summary>
    public async Task<ArticleEditData> GetArticleEditDataAsync(int articleId, CancellationToken cancellationToken = default)
    {
        if (articleId <= 0)
        {
            return new ArticleEditData(string.Empty, 0, string.Empty);
        }

        var html = await _api.GetTextAsync($"/Admin/Article/Edit/{articleId}", cancellationToken: cancellationToken);

        var title = string.Empty;
        var inputTag = TitleInputRegex.Match(html);
        if (inputTag.Success)
        {
            var value = ValueAttrRegex.Match(inputTag.Value);
            title = value.Success ? WebUtility.HtmlDecode(value.Groups[1].Value) : string.Empty;
        }

        var contents = string.Empty;
        var textarea = ContentsRegex.Match(html);
        if (textarea.Success)
        {
            contents = WebUtility.HtmlDecode(textarea.Groups[1].Value);
        }

        var typeId = 0;
        var select = ArticleTypeSelectRegex.Match(html);
        if (select.Success)
        {
            var selected = SelectedOptionRegex.Match(select.Groups[1].Value);
            if (selected.Success && int.TryParse(selected.Groups[1].Value, out var parsed))
            {
                typeId = parsed;
            }
        }

        return new ArticleEditData(title, typeId, contents);
    }

    /// <summary>解析首页设置页中的展示方式与已选分类顺序。</summary>
    public async Task<HomeSectionsData> GetHomeSectionsAsync(CancellationToken cancellationToken = default)
    {
        var html = await _api.GetTextAsync("/Admin/HomeSections/Index", cancellationToken: cancellationToken);
        var mode = CategoryModeRegex.IsMatch(html) ? "category" : "flags";

        var categoryIds = new List<int>();
        var block = SelectedTypesBlockRegex.Match(html);
        if (block.Success)
        {
            foreach (Match match in TypeIdAttrRegex.Matches(block.Groups[1].Value))
            {
                if (int.TryParse(match.Groups[1].Value, out var parsed) && parsed > 0 && !categoryIds.Contains(parsed))
                {
                    categoryIds.Add(parsed);
                }
            }
        }

        // flags 模式在页面上没有 checked 属性时由默认值兜底
        if (!FlagsModeRegex.IsMatch(html) && !CategoryModeRegex.IsMatch(html))
        {
            mode = "flags";
        }

        return new HomeSectionsData(mode, categoryIds);
    }
}
