using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>AI 设置示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class AiSettingsController : Controller
{
    /// <summary>模型设置。</summary>
    /// <returns>模型视图。</returns>
    [HttpGet]
    public IActionResult Models() => Render("Models", "模型設定", "ai-models", "Models：可用模型、預設與啟停");

    /// <summary>密钥管理。</summary>
    /// <returns>密钥视图。</returns>
    [HttpGet]
    public IActionResult ApiKeys() => Render("ApiKeys", "API 金鑰", "ai-api-keys", "API Keys：建立、遮罩與撤銷");

    /// <summary>用量统计。</summary>
    /// <returns>用量视图。</returns>
    [HttpGet]
    public IActionResult Usage() => Render("Usage", "用量統計", "ai-usage", "Usage：Token、費用與模型分佈");

    /// <summary>提示词库。</summary>
    /// <returns>提示词视图。</returns>
    [HttpGet]
    public IActionResult Prompts() => Render("Prompts", "提示詞庫", "ai-prompts", "Prompt Library：常用提示詞與一鍵複製");

    /// <summary>安全与限制。</summary>
    /// <returns>安全设置视图。</returns>
    [HttpGet]
    public IActionResult Safety() => Render("Safety", "安全與限制", "ai-safety", "Safety：內容過濾、速率限制與敏感詞");

    /// <summary>对话记录。</summary>
    /// <returns>记录视图。</returns>
    [HttpGet]
    public IActionResult History() => Render("History", "對話記錄", "ai-history", "History：會話、模型與 Token 消耗");

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "AI 設定", title };
        return View(viewName);
    }
}
