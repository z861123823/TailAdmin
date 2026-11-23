using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>文章管理。</summary>
[Area("Admin")]
public sealed class ArticleController : AlsControllerBase
{
    private readonly AlsAdminGateway _gateway;

    /// <summary>构造函数。</summary>
    public ArticleController(AlsApiClient api, AlsAdminGateway gateway, ILogger<ArticleController> logger)
        : base(api, logger)
    {
        _gateway = gateway;
    }

    /// <summary>文章列表页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "文章管理";
        ViewData["Subtitle"] = "對接商城文章接口，支援新增、編輯與刪除";
        ViewData["MenuKey"] = "article";
        ViewData["Breadcrumb"] = new[] { "後台", "內容中心", "文章管理" };
        return View();
    }

    /// <summary>文章列表。</summary>
    [HttpGet]
    public Task<IActionResult> List(CancellationToken cancellationToken)
        => ForwardListAsync("/Admin/Article/List", new Dictionary<string, string?>(), cancellationToken);

    /// <summary>文章编辑弹窗。</summary>
    [HttpGet]
    public async Task<IActionResult> EditForm(int id, CancellationToken cancellationToken)
    {
        var data = await _gateway.GetArticleEditDataAsync(id, cancellationToken);
        var model = new ArticleFormModel
        {
            Id = id,
            Title = data.Title,
            TypeId = data.TypeId,
            Contents = data.Contents,
            Types = await _gateway.GetArticleTypesAsync(cancellationToken)
        };
        return PartialView("_Form", model);
    }

    /// <summary>保存文章。</summary>
    [HttpPost]
    public Task<IActionResult> Save(int id, int typeId, string? title, string? contents, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Article/Save", new[]
        {
            Pair("id", id),
            Pair("typeId", typeId),
            Pair("title", title),
            Pair("contents", contents)
        }, cancellationToken);

    /// <summary>删除文章。</summary>
    [HttpPost]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Article/Delete", new[] { Pair("id", id) }, cancellationToken);

    private static KeyValuePair<string, string?> Pair(string key, object? value) => new(key, value?.ToString());
}
