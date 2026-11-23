using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>AI 能力示例页。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class AiController : Controller
{
    /// <summary>AI 助手对话。</summary>
    /// <returns>对话视图。</returns>
    [HttpGet]
    public IActionResult Chat() => Render("Chat", "AI 助手", "ai-chat", "AI Chat：對話、建議與輸入");

    /// <summary>文本生成。</summary>
    /// <returns>文本生成视图。</returns>
    [HttpGet]
    public IActionResult TextGenerator() => Render("TextGenerator", "文本生成", "ai-text", "Text Generator：提示詞、語氣與結果");

    /// <summary>图片生成。</summary>
    /// <returns>图片生成视图。</returns>
    [HttpGet]
    public IActionResult ImageGenerator() => Render("ImageGenerator", "圖片生成", "ai-image", "Image Generator：提示詞、風格與尺寸");

    /// <summary>代码生成。</summary>
    /// <returns>代码生成视图。</returns>
    [HttpGet]
    public IActionResult CodeGenerator() => Render("CodeGenerator", "代碼生成", "ai-code", "Code Generator：語言、框架與代碼結果");

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "AI", title };
        return View(viewName);
    }
}
