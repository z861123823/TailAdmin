using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using TailAdmin.Demo.Web.Models;
using TailAdmin.Demo.Web.Services;

namespace TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>商品管理：列表 / 弹窗表单 / 保存 / 删除 / 图片上传。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class GoodsController : Controller
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private readonly DemoDataStore _store;
    private readonly IWebHostEnvironment _environment;

    /// <summary>初始化商品控制器。</summary>
    /// <param name="store">演示数据仓储。</param>
    /// <param name="environment">宿主环境。</param>
    public GoodsController(DemoDataStore store, IWebHostEnvironment environment)
    {
        _store = store;
        _environment = environment;
    }

    /// <summary>商品列表页。</summary>
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "商品管理";
        ViewData["Subtitle"] = "支援服務端分頁、排序、關鍵字查詢與彈窗編輯";
        ViewData["MenuKey"] = "goods";
        ViewData["Breadcrumb"] = new[] { "後台", "商品中心", "商品管理" };
        ViewData["UseDatepicker"] = true;
        return View();
    }

    /// <summary>列表接口：{ code, msg, count, data }。</summary>
    [HttpGet]
    public IActionResult List(int page = 1, int limit = 20, string? sortField = null, string? sortOrder = "desc", string? keyword = null, int? typeId = null)
    {
        page = Math.Max(1, page);
        limit = Math.Clamp(limit, 1, 200);
        var (items, total) = _store.QueryGoods(page, limit, sortField, sortOrder, keyword, typeId);
        return Json(ApiResults.Table(items, total));
    }

    /// <summary>弹窗表单 Partial。</summary>
    [HttpGet]
    public IActionResult EditForm(int id = 0)
    {
        GoodsEditModel model;
        if (id > 0)
        {
            var item = _store.GetGoods(id);
            if (item is null)
            {
                return NotFound();
            }

            model = new GoodsEditModel
            {
                Id = item.Id,
                GoodsName = item.GoodsName,
                PicUrl = item.PicUrl,
                Price = item.Price,
                TypeId = item.TypeId,
                Tags = item.Tags.ToList(),
                SalesVolume = item.SalesVolume,
                IsHot = item.IsHot,
                AddTime = item.AddTime.ToString("yyyy-MM-dd"),
                Remark = item.Remark
            };
        }
        else
        {
            model = new GoodsEditModel
            {
                TypeId = _store.GetTypes().FirstOrDefault()?.Id ?? 0,
                AddTime = DateTime.Today.ToString("yyyy-MM-dd")
            };
        }

        ViewBag.TypeList = BuildTypeOptions();
        return PartialView("_Form", model);
    }

    /// <summary>保存商品。</summary>
    [HttpPost]
    public IActionResult Save(GoodsEditModel model)
    {
        if (model.TypeId > 0 && _store.GetTypes().All(type => type.Id != model.TypeId))
        {
            ModelState.AddModelError(nameof(model.TypeId), "商品分類不存在");
        }

        if (!ModelState.IsValid)
        {
            return Json(ApiResults.Fail("參數校驗失敗", BuildErrors(ModelState)));
        }

        var isNew = model.Id <= 0;
        var saved = _store.SaveGoods(model);
        return Json(ApiResults.Ok(isNew ? "新增成功" : "保存成功", new { saved.Id }));
    }

    /// <summary>删除商品。</summary>
    [HttpPost]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return Json(ApiResults.Fail("參數不正確"));
        }

        return Json(_store.DeleteGoods(id) ? ApiResults.Ok("刪除成功") : ApiResults.Fail("商品不存在或已刪除"));
    }

    /// <summary>图片上传（multipart 与 Base64 兼容通道）。</summary>
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile? file, string? base64)
    {
        const long maxFileSize = 2 * 1024 * 1024;
        const long maxBase64Size = 512 * 1024;

        byte[] buffer;
        string extension;

        if (file is { Length: > 0 })
        {
            if (file.Length > maxFileSize)
            {
                return Json(ApiResults.Fail("文件超過 2MB 限制"));
            }

            extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return Json(ApiResults.Fail("僅支援 jpg / png / webp / gif"));
            }

            using var memory = new MemoryStream();
            await file.CopyToAsync(memory);
            buffer = memory.ToArray();
        }
        else if (!string.IsNullOrWhiteSpace(base64))
        {
            var commaIndex = base64.IndexOf(',');
            if (commaIndex < 0 || !base64.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            {
                return Json(ApiResults.Fail("圖片數據格式不正確"));
            }

            var meta = base64[..commaIndex];
            extension = meta switch
            {
                var m when m.Contains("png", StringComparison.OrdinalIgnoreCase) => ".png",
                var m when m.Contains("webp", StringComparison.OrdinalIgnoreCase) => ".webp",
                var m when m.Contains("gif", StringComparison.OrdinalIgnoreCase) => ".gif",
                _ => ".jpg"
            };

            try
            {
                buffer = Convert.FromBase64String(base64[(commaIndex + 1)..]);
            }
            catch (FormatException)
            {
                return Json(ApiResults.Fail("圖片數據解碼失敗"));
            }

            if (buffer.Length > maxBase64Size)
            {
                return Json(ApiResults.Fail("Base64 小圖不能超過 512KB"));
            }
        }
        else
        {
            return Json(ApiResults.Fail("請選擇要上傳的圖片"));
        }

        if (!IsValidImage(buffer, extension))
        {
            return Json(ApiResults.Fail("文件內容與圖片格式不符"));
        }

        var folder = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "Uploads", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
        Directory.CreateDirectory(folder);
        var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(folder, fileName);
        await System.IO.File.WriteAllBytesAsync(fullPath, buffer);

        var picUrl = $"/Uploads/{DateTime.Now:yyyy}/{DateTime.Now:MM}/{fileName}";
        return Json(ApiResults.Ok("上傳成功", new
        {
            PicId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            PicName = fileName,
            PicUrl = picUrl
        }));
    }

    /// <summary>标签远程搜索（taMultiSelect 数据源）。</summary>
    [HttpGet]
    public IActionResult TagSearch(string? keyword = null, string? ids = null)
    {
        var idList = string.IsNullOrWhiteSpace(ids) ? null : ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var tags = _store.SearchTags(keyword, idList);
        return Json(new
        {
            Success = true,
            Message = string.Empty,
            Data = tags.Select(tag => new { Value = tag.Value, Label = tag.Label, Disabled = false })
        });
    }

    private List<SelectListItem> BuildTypeOptions()
        => _store.GetTypes().Select(type => new SelectListItem(type.Name, type.Id.ToString())).ToList();

    private static Dictionary<string, string[]> BuildErrors(ModelStateDictionary modelState)
        => modelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

    private static bool IsValidImage(byte[] buffer, string extension)
    {
        if (buffer.Length < 12)
        {
            return false;
        }

        return extension switch
        {
            ".png" => buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47,
            ".jpg" or ".jpeg" => buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF,
            ".gif" => buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46,
            ".webp" => buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46,
            _ => false
        };
    }
}
