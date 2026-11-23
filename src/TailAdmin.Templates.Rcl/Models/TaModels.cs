using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TailAdmin.Templates.Rcl.Models;

/// <summary>表格列定义。</summary>
public sealed class TaColumnModel
{
    /// <summary>数据字段名（PascalCase，与接口一致）。</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>表头文案。</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>列类型：text / number / money / date / bool / tag / image / link。</summary>
    public string Type { get; set; } = "text";

    /// <summary>列宽（px）。</summary>
    public int? Width { get; set; }

    /// <summary>对齐方式：left / center / right。</summary>
    public string? Align { get; set; }

    /// <summary>是否可排序（需服务端白名单支持）。</summary>
    public bool Sortable { get; set; } = true;

    /// <summary>自定义格式化函数名（注册在 Ta.format 中）。</summary>
    public string? Formatter { get; set; }

    /// <summary>单元格附加类名（仅允许主题类）。</summary>
    public string? CellClass { get; set; }
}

/// <summary>行操作按钮。</summary>
public sealed class TaActionModel
{
    /// <summary>按钮文案。</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>事件名，对应 onAction 回调表中的键。</summary>
    public string Event { get; set; } = string.Empty;

    /// <summary>样式：primary / error / ghost / outline。</summary>
    public string? Variant { get; set; }

    /// <summary>是否显示。</summary>
    public bool Visible { get; set; } = true;
}

/// <summary>
/// taTable 的 Razor 配置模型，配合 Partial 渲染统一表格结构。
/// </summary>
public sealed class TaTableModel
{
    /// <summary>表格标识，与查询弹窗的 tableId 对应。</summary>
    public string Id { get; set; } = "taTable";

    /// <summary>列表接口地址。</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>请求方式：GET（默认）/ POST。</summary>
    public string Method { get; set; } = "GET";

    /// <summary>行主键字段名。</summary>
    public string RowKey { get; set; } = "Id";

    /// <summary>每页条数。</summary>
    public int PageSize { get; set; } = 20;

    /// <summary>是否显示多选列。</summary>
    public bool Selectable { get; set; }

    /// <summary>空态文案。</summary>
    public string? EmptyText { get; set; }

    /// <summary>列定义。</summary>
    public List<TaColumnModel> Columns { get; set; } = new();

    /// <summary>行操作。</summary>
    public List<TaActionModel> Actions { get; set; } = new();

    /// <summary>固定筛选参数。</summary>
    public Dictionary<string, object?>? Params { get; set; }

    /// <summary>提交成功后需要自动刷新的弹窗标识。</summary>
    public List<string>? ReloadOn { get; set; }

    /// <summary>
    /// onAction 的 JavaScript 对象字面量内容（不含外层花括号），
    /// 例如：<c>edit: function(row){ Ta.modal.load('goodsEdit', '/Admin/Goods/EditForm', { Id: row.Id }); }</c>
    /// </summary>
    public string? OnAction { get; set; }

    /// <summary>生成 taTable 的初始化配置 JSON。</summary>
    /// <returns>可直接嵌入 x-data 的 JSON 字符串。</returns>
    public string BuildConfigJson()
    {
        var payload = new Dictionary<string, object?>
        {
            ["id"] = Id,
            ["url"] = Url,
            ["method"] = Method,
            ["rowKey"] = RowKey,
            ["pageSize"] = PageSize,
            ["selectable"] = Selectable,
            ["emptyText"] = EmptyText,
            ["params"] = Params,
            ["reloadOn"] = ReloadOn,
            ["columns"] = Columns.Select(column => new Dictionary<string, object?>
            {
                ["key"] = column.Key,
                ["label"] = column.Label,
                ["type"] = column.Type,
                ["width"] = column.Width,
                ["align"] = column.Align,
                ["sortable"] = column.Sortable,
                ["formatter"] = column.Formatter,
                ["cellClass"] = column.CellClass
            }).ToList(),
            ["actions"] = Actions.Select(action => new Dictionary<string, object?>
            {
                ["label"] = action.Label,
                ["event"] = action.Event,
                ["variant"] = action.Variant,
                ["visible"] = action.Visible
            }).ToList()
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(payload, options);
        if (string.IsNullOrWhiteSpace(OnAction))
        {
            return json;
        }

        return json.TrimEnd('}') + ",\"onAction\":{" + OnAction + "}}";
    }
}

/// <summary>列表页工具栏模型。</summary>
public sealed class TableToolbarModel
{
    /// <summary>关联的表格标识。</summary>
    public string TableId { get; set; } = "taTable";

    /// <summary>工具栏左侧说明文字。</summary>
    public string? Description { get; set; }

    /// <summary>查询输入框占位文本。</summary>
    public string? Placeholder { get; set; }

    /// <summary>新增按钮文案；为空时不显示新增按钮。</summary>
    public string? CreateText { get; set; }

    /// <summary>新增按钮打开的弹窗标识。</summary>
    public string? CreateModalId { get; set; }

    /// <summary>新增表单 Partial 地址。</summary>
    public string? CreateUrl { get; set; }
}

/// <summary>页面标题模型。</summary>
public sealed class PageHeaderModel
{
    /// <summary>标题。</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>副标题。</summary>
    public string? Subtitle { get; set; }

    /// <summary>面包屑。</summary>
    public IReadOnlyList<string> Breadcrumb { get; set; } = Array.Empty<string>();
}

/// <summary>空态模型。</summary>
public sealed class EmptyStateModel
{
    /// <summary>标题。</summary>
    public string Title { get; set; } = "暫無數據";

    /// <summary>说明文字。</summary>
    public string? Description { get; set; }

    /// <summary>主操作文案；为空时不显示按钮。</summary>
    public string? ActionText { get; set; }

    /// <summary>主操作链接。</summary>
    public string? ActionUrl { get; set; }
}
