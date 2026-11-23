using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>轮播图管理。</summary>
[Area("Admin")]
public sealed class SlidesController : AlsControllerBase
{
    /// <summary>构造函数。</summary>
    public SlidesController(AlsApiClient api, ILogger<SlidesController> logger) : base(api, logger)
    {
    }

    /// <summary>轮播图列表页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "輪播圖管理";
        ViewData["Subtitle"] = "對接商城 /Admin/Slides 接口，圖片支援直接上傳與替換";
        ViewData["MenuKey"] = "slides";
        ViewData["Breadcrumb"] = new[] { "後台", "內容中心", "輪播圖" };
        return View();
    }

    /// <summary>轮播图列表。</summary>
    [HttpGet]
    public Task<IActionResult> List(CancellationToken cancellationToken)
        => ForwardListAsync("/Admin/Slides/List", new Dictionary<string, string?>(), cancellationToken);

    /// <summary>轮播图编辑弹窗。</summary>
    [HttpGet]
    public IActionResult EditForm(int id, string? picName, string? picUrl, string? picUrls)
    {
        var preview = !string.IsNullOrWhiteSpace(picUrls)
            ? picUrls
            : !string.IsNullOrWhiteSpace(picName)
                ? "/mall-assets/images/Slides/" + picName
                : null;

        return PartialView("_Form", new SlideFormModel
        {
            Id = id,
            PicName = picName,
            PicUrl = picUrl,
            PreviewUrl = preview
        });
    }

    /// <summary>保存轮播图。</summary>
    [HttpPost]
    public Task<IActionResult> Save(int id, string? picName, string? picUrl, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Slides/Save", new[]
        {
            new KeyValuePair<string, string?>("id", id.ToString()),
            new KeyValuePair<string, string?>("picName", picName),
            new KeyValuePair<string, string?>("picUrl", picUrl)
        }, cancellationToken);

    /// <summary>删除轮播图。</summary>
    [HttpPost]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Slides/Delete", new[]
        {
            new KeyValuePair<string, string?>("id", id.ToString())
        }, cancellationToken);
}
