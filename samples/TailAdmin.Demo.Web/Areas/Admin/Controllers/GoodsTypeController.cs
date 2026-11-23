using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TailAdmin.Demo.Web.Models;
using TailAdmin.Demo.Web.Services;

namespace TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>商品分类：一期迁移试点页（标准 CRUD + 弹窗）。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class GoodsTypeController : Controller
{
    private readonly DemoDataStore _store;

    /// <summary>初始化分类控制器。</summary>
    /// <param name="store">演示数据仓储。</param>
    public GoodsTypeController(DemoDataStore store)
    {
        _store = store;
    }

    /// <summary>分类列表页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "商品分類";
        ViewData["Subtitle"] = "標準 CRUD 試點頁，與商品頁共用同一套表格與彈窗";
        ViewData["MenuKey"] = "goods-type";
        ViewData["Breadcrumb"] = new[] { "後台", "商品中心", "商品分類" };
        return View();
    }

    /// <summary>列表接口。</summary>
    [HttpGet]
    public IActionResult List(int page = 1, int limit = 20, string? sortField = null, string? sortOrder = "desc", string? keyword = null)
    {
        page = Math.Max(1, page);
        limit = Math.Clamp(limit, 1, 200);
        var (items, total) = _store.QueryTypes(page, limit, keyword);
        return Json(ApiResults.Table(items, total));
    }

    /// <summary>弹窗表单 Partial。</summary>
    [HttpGet]
    public IActionResult EditForm(int id = 0)
    {
        if (id <= 0)
        {
            return PartialView("_Form", new GoodsTypeEditModel { Enabled = true, Sort = 100 });
        }

        var item = _store.GetType(id);
        if (item is null)
        {
            return NotFound();
        }

        return PartialView("_Form", new GoodsTypeEditModel
        {
            Id = item.Id,
            Name = item.Name,
            Sort = item.Sort,
            Enabled = item.Enabled
        });
    }

    /// <summary>保存分类。</summary>
    [HttpPost]
    public IActionResult Save(GoodsTypeEditModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(entry => entry.Key, entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());
            return Json(ApiResults.Fail("參數校驗失敗", errors));
        }

        var isNew = model.Id <= 0;
        var saved = _store.SaveType(model);
        return Json(ApiResults.Ok(isNew ? "新增成功" : "保存成功", new { saved.Id }));
    }

    /// <summary>删除分类。</summary>
    [HttpPost]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return Json(ApiResults.Fail("參數不正確"));
        }

        var error = _store.DeleteType(id);
        return Json(error is null ? ApiResults.Ok("刪除成功") : ApiResults.Fail(error));
    }
}
