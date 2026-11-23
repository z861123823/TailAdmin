using System.Text.Json.Nodes;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>测试后台控制器基类：统一转发商城接口、改写图片地址并输出 JSON。</summary>
public abstract class AlsControllerBase : Controller
{
    private readonly ILogger _logger;

    /// <summary>构造函数。</summary>
    protected AlsControllerBase(AlsApiClient api, ILogger logger)
    {
        Api = api;
        _logger = logger;
    }

    /// <summary>商城接口客户端。</summary>
    protected AlsApiClient Api { get; }

    /// <summary>转发列表查询（商城返回 layui 表格格式的 count/data，与前端表格组件一致）。</summary>
    protected async Task<IActionResult> ForwardListAsync(
        string path,
        IDictionary<string, string?> query,
        CancellationToken cancellationToken)
    {
        try
        {
            var node = await Api.GetJsonAsync(path, query, cancellationToken);
            AlsJson.PrefixAssetUrls(node);
            return Content(AlsJson.ToJson(node), "application/json; charset=utf-8");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "调用商城列表接口失败：{Path}", path);
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                code = 1,
                msg = "商城接口调用失败：" + exception.Message,
                count = 0,
                data = Array.Empty<object>()
            });
        }
    }

    /// <summary>转发写操作（自动附加防伪令牌）。</summary>
    protected async Task<IActionResult> ForwardPostAsync(
        string path,
        IEnumerable<KeyValuePair<string, string?>> fields,
        CancellationToken cancellationToken)
    {
        try
        {
            var body = await Api.PostFormAsync(path, fields, cancellationToken);
            return string.IsNullOrWhiteSpace(body)
                ? Json(new { Success = "false", Message = "商城接口未返回内容" })
                : Content(body, "application/json; charset=utf-8");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "调用商城写接口失败：{Path}", path);
            return Json(new { Success = "false", Message = "商城接口调用失败：" + exception.Message });
        }
    }

    /// <summary>转发 GET 并返回带图片地址改写的 JSON。</summary>
    protected async Task<IActionResult> ForwardJsonAsync(
        string path,
        IDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        try
        {
            var node = await Api.GetJsonAsync(path, query, cancellationToken);
            AlsJson.PrefixAssetUrls(node);
            return Content(AlsJson.ToJson(node), "application/json; charset=utf-8");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "调用商城接口失败：{Path}", path);
            return Json(new { Success = "false", Message = "商城接口调用失败：" + exception.Message });
        }
    }

    /// <summary>把 JSON 节点输出为响应。</summary>
    protected IActionResult JsonNodeResult(JsonNode? node)
    {
        AlsJson.PrefixAssetUrls(node);
        return Content(AlsJson.ToJson(node), "application/json; charset=utf-8");
    }

    /// <summary>失败结果。</summary>
    protected IActionResult Fail(string message) => Json(new { Success = "false", Message = message });
}
