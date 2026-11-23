using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>文章分类管理。</summary>
[Area("Admin")]
public sealed class ArticleTypeController : AlsControllerBase
{
    /// <summary>构造函数。</summary>
    public ArticleTypeController(AlsApiClient api, ILogger<ArticleTypeController> logger) : base(api, logger)
    {
    }

    /// <summary>分类列表页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "文章分類";
        ViewData["Subtitle"] = "文章分類的新增、修改與刪除，刪除前商城會檢查是否有文章佔用";
        ViewData["MenuKey"] = "article-type";
        ViewData["Breadcrumb"] = new[] { "後台", "內容中心", "文章分類" };
        return View();
    }

    /// <summary>分类列表。</summary>
    [HttpGet]
    public Task<IActionResult> List(CancellationToken cancellationToken)
        => ForwardListAsync("/Admin/ArticleType/List", new Dictionary<string, string?>(), cancellationToken);

    /// <summary>分类编辑弹窗。</summary>
    [HttpGet]
    public IActionResult EditForm(int id, string? typeName)
        => PartialView("_Form", new ArticleTypeFormModel { Id = id, TypeName = typeName });

    /// <summary>保存分类。</summary>
    [HttpPost]
    public Task<IActionResult> Save(int id, string? typeName, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/ArticleType/Save", new[]
        {
            new KeyValuePair<string, string?>("id", id.ToString()),
            new KeyValuePair<string, string?>("typeName", typeName)
        }, cancellationToken);

    /// <summary>删除分类。</summary>
    [HttpPost]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/ArticleType/Delete", new[]
        {
            new KeyValuePair<string, string?>("id", id.ToString())
        }, cancellationToken);
}
