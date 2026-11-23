using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>电子商务示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class EcommerceController : Controller
{
    /// <summary>商品列表。</summary>
    /// <returns>商品列表视图。</returns>
    [HttpGet]
    public IActionResult Products() => Render("Products", "商品列表", "ecommerce-products", "商品列表：查詢、分頁與狀態");

    /// <summary>新增商品。</summary>
    /// <returns>新增商品视图。</returns>
    [HttpGet]
    public IActionResult AddProduct() => Render("AddProduct", "新增商品", "ecommerce-add", "新增商品：基本資訊、價格與發佈設定");

    /// <summary>账单信息。</summary>
    /// <returns>账单视图。</returns>
    [HttpGet]
    public IActionResult Billing() => Render("Billing", "帳單", "ecommerce-billing", "Billing：方案、付款方式與帳單地址");

    /// <summary>发票列表。</summary>
    /// <returns>发票列表视图。</returns>
    [HttpGet]
    public IActionResult Invoices() => Render("Invoices", "發票", "ecommerce-invoices", "Invoices：開票記錄與下載");

    /// <summary>发票详情。</summary>
    /// <param name="id">发票编号。</param>
    /// <returns>发票详情视图。</returns>
    [HttpGet]
    public IActionResult InvoiceDetail(string? id)
    {
        ViewData["InvoiceNo"] = string.IsNullOrWhiteSpace(id) ? "INV-2026-0042" : id;
        return Render("InvoiceDetail", "發票詳情", "ecommerce-invoices", "Invoice Detail：明細、稅額與付款資訊");
    }

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "電子商務", title };
        return View(viewName);
    }
}
