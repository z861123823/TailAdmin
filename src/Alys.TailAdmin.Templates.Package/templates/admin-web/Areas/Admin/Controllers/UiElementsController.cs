using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

/// <summary>UI 元件示例页，按官方 TailAdmin 的 UI Elements 目录逐组件拆分。</summary>
[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class UiElementsController : Controller
{
    /// <summary>提示 Alert。</summary>
    /// <returns>提示示例视图。</returns>
    [HttpGet]
    public IActionResult Alerts() => Render("Alerts", "提示", "ui-alerts", "Alert：Default / Success / Warning / Error / Info");

    /// <summary>头像 Avatar。</summary>
    /// <returns>头像示例视图。</returns>
    [HttpGet]
    public IActionResult Avatars() => Render("Avatars", "頭像", "ui-avatars", "Avatar / AvatarGroup：尺寸、狀態與重疊分組");

    /// <summary>徽章 Badge。</summary>
    /// <returns>徽章示例视图。</returns>
    [HttpGet]
    public IActionResult Badges() => Render("Badges", "徽章", "ui-badges", "Badge：色彩、尺寸、圓點與圖標");

    /// <summary>面包屑 Breadcrumb。</summary>
    /// <returns>面包屑示例视图。</returns>
    [HttpGet]
    public IActionResult Breadcrumb() => Render("Breadcrumb", "麵包屑", "ui-breadcrumb", "Breadcrumb：首頁入口與當前頁標識");

    /// <summary>按钮 Button。</summary>
    /// <returns>按钮示例视图。</returns>
    [HttpGet]
    public IActionResult Buttons() => Render("Buttons", "按鈕", "ui-buttons", "Button：樣式、尺寸、圖標與禁用態");

    /// <summary>按钮组 ButtonGroup。</summary>
    /// <returns>按钮组示例视图。</returns>
    [HttpGet]
    public IActionResult ButtonGroup() => Render("ButtonGroup", "按鈕組", "ui-button-group", "ButtonGroup：分段式操作與尺寸");

    /// <summary>卡片 Card。</summary>
    /// <returns>卡片示例视图。</returns>
    [HttpGet]
    public IActionResult Cards() => Render("Cards", "卡片", "ui-cards", "Card：標題、連結、底欄與自定義內容");

    /// <summary>轮播 Carousel。</summary>
    /// <returns>轮播示例视图。</returns>
    [HttpGet]
    public IActionResult Carousel() => Render("Carousel", "輪播", "ui-carousel", "Carousel：左右切換與指示點");

    /// <summary>下拉菜单 Dropdown。</summary>
    /// <returns>下拉菜单示例视图。</returns>
    [HttpGet]
    public IActionResult Dropdowns() => Render("Dropdowns", "下拉選單", "ui-dropdowns", "Dropdown：對齊方式、圖標與危險項");

    /// <summary>图片 Images。</summary>
    /// <returns>图片示例视图。</returns>
    [HttpGet]
    public IActionResult Images() => Render("Images", "圖片", "ui-images", "Images：列數、比例與說明文字");

    /// <summary>链接 Links。</summary>
    /// <returns>链接示例视图。</returns>
    [HttpGet]
    public IActionResult Links() => Render("Links", "連結", "ui-links", "Link：主要、次要、下劃線與外鏈");

    /// <summary>列表 List。</summary>
    /// <returns>列表示例视图。</returns>
    [HttpGet]
    public IActionResult Lists() => Render("Lists", "列表", "ui-lists", "List：圖標、描述、徽章與當前項");

    /// <summary>弹窗 Modal。</summary>
    /// <returns>弹窗示例视图。</returns>
    [HttpGet]
    public IActionResult Modals() => Render("Modals", "彈窗", "ui-modals", "Modal：基礎彈窗、確認彈窗與頁腳操作");

    /// <summary>分页 Pagination。</summary>
    /// <returns>分页示例视图。</returns>
    [HttpGet]
    public IActionResult Pagination() => Render("Pagination", "分頁", "ui-pagination", "Pagination：頁碼、上一頁與下一頁");

    /// <summary>进度条 Progress。</summary>
    /// <returns>进度条示例视图。</returns>
    [HttpGet]
    public IActionResult Progress() => Render("Progress", "進度條", "ui-progress", "Progress：線性、環形、尺寸與色彩");

    /// <summary>标签页 Tabs。</summary>
    /// <returns>标签页示例视图。</returns>
    [HttpGet]
    public IActionResult Tabs() => Render("Tabs", "標籤頁", "ui-tabs", "Tabs：下劃線、膠囊與圖標樣式");

    /// <summary>工具提示 Tooltip。</summary>
    /// <returns>工具提示示例视图。</returns>
    [HttpGet]
    public IActionResult Tooltips() => Render("Tooltips", "工具提示", "ui-tooltips", "Tooltip：四個方向與色彩變體");

    /// <summary>弹出层 Popover。</summary>
    /// <returns>弹出层示例视图。</returns>
    [HttpGet]
    public IActionResult Popovers() => Render("Popovers", "彈出層", "ui-popovers", "Popover：點擊觸發、帶標題與操作");

    /// <summary>加载指示 Spinner。</summary>
    /// <returns>加载指示示例视图。</returns>
    [HttpGet]
    public IActionResult Spinners() => Render("Spinners", "加載指示", "ui-spinners", "Spinner：圓環、點狀、條狀與按鈕加載");

    /// <summary>角标 Ribbon。</summary>
    /// <returns>角标示例视图。</returns>
    [HttpGet]
    public IActionResult Ribbons() => Render("Ribbons", "角標", "ui-ribbons", "Ribbon：卡片角標與促銷標記");

    /// <summary>视频 Video。</summary>
    /// <returns>视频示例视图。</returns>
    [HttpGet]
    public IActionResult Videos() => Render("Videos", "影片", "ui-videos", "Video：播放器、封面與縮略圖");

    /// <summary>通知 Notification。</summary>
    /// <returns>通知示例视图。</returns>
    [HttpGet]
    public IActionResult Notifications() => Render("Notifications", "通知", "ui-notifications", "Notification：列表、未讀與浮層提示");

    private IActionResult Render(string viewName, string title, string menuKey, string subtitle)
    {
        ViewData["Title"] = title;
        ViewData["Subtitle"] = subtitle;
        ViewData["MenuKey"] = menuKey;
        ViewData["Breadcrumb"] = new[] { "後台", "UI 元件", title };
        return View(viewName);
    }
}

