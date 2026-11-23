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

    /// <summary>
    /// 固定列位置：null / "none" 不固定，"left" 吸附左侧，"right" 吸附右侧。
    /// 左侧固定列须为最左的连续列，右侧固定列须为最右的连续列。
    /// </summary>
    public string? Fixed { get; set; }
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

    /// <summary>是否固定多选列（吸附左侧）。</summary>
    public bool FixedSelectable { get; set; }

    /// <summary>是否固定操作列（吸附右侧）。</summary>
    public bool FixedActions { get; set; }

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

    private const int SelectableColumnWidth = 48;
    private const int ActionColumnWidth = 112;
    private const int DefaultFixedColumnWidth = 160;

    /// <summary>是否存在可见的操作按钮。</summary>
    public bool HasVisibleActions => Actions.Any(action => action.Visible);

    /// <summary>列布局（含固定位置、宽度与偏移量），供视图表头与前端渲染复用。</summary>
    public IReadOnlyList<TableColumnLayout> ColumnLayout => BuildColumnLayout();

    /// <summary>计算列固定偏移量：左侧从 0 累加，右侧从最右端向左累加。</summary>
    public List<TableColumnLayout> BuildColumnLayout()
    {
        var items = new List<TableColumnLayout>(Columns.Count);
        var left = FixedSelectable ? SelectableColumnWidth : 0;

        foreach (var column in Columns)
        {
            var fixedLeft = string.Equals(column.Fixed, "left", StringComparison.OrdinalIgnoreCase);
            var fixedRight = string.Equals(column.Fixed, "right", StringComparison.OrdinalIgnoreCase);
            var width = column.Width ?? ((fixedLeft || fixedRight) ? DefaultFixedColumnWidth : (int?)null);
            var item = new TableColumnLayout
            {
                Column = column,
                FixedLeft = fixedLeft,
                FixedRight = fixedRight,
                Width = width,
                Left = fixedLeft ? left : null
            };
            if (fixedLeft)
            {
                left += width ?? DefaultFixedColumnWidth;
            }

            items.Add(item);
        }

        // 右侧固定：操作列在最右，右固定数据列从右往左累加偏移
        var right = (HasVisibleActions && FixedActions) ? ActionColumnWidth : 0;
        for (var index = items.Count - 1; index >= 0; index--)
        {
            var item = items[index];
            if (!item.FixedRight)
            {
                continue;
            }

            item.Right = right;
            right += item.Width ?? DefaultFixedColumnWidth;
        }

        // 边缘标记：最靠中间的左固定列 / 右固定列承载滚动阴影
        var lastLeft = items.LastOrDefault(item => item.FixedLeft);
        if (lastLeft is not null)
        {
            lastLeft.EdgeLeft = true;
        }

        var firstRight = items.FirstOrDefault(item => item.FixedRight);
        if (firstRight is not null)
        {
            firstRight.EdgeRight = true;
        }

        return items;
    }

    /// <summary>生成 taTable 的初始化配置 JSON。</summary>
    /// <returns>可直接嵌入 x-data 的 JSON 字符串。</returns>
    public string BuildConfigJson()
    {
        var layout = BuildColumnLayout();
        var payload = new Dictionary<string, object?>
        {
            ["id"] = Id,
            ["url"] = Url,
            ["method"] = Method,
            ["rowKey"] = RowKey,
            ["pageSize"] = PageSize,
            ["selectable"] = Selectable,
            ["fixedSelectable"] = FixedSelectable,
            ["fixedActions"] = HasVisibleActions && FixedActions,
            ["emptyText"] = EmptyText,
            ["params"] = Params,
            ["reloadOn"] = ReloadOn,
            ["columns"] = layout.Select(item => new Dictionary<string, object?>
            {
                ["key"] = item.Column.Key,
                ["label"] = item.Column.Label,
                ["type"] = item.Column.Type,
                ["width"] = item.Width,
                ["align"] = item.Column.Align,
                ["sortable"] = item.Column.Sortable,
                ["formatter"] = item.Column.Formatter,
                ["cellClass"] = item.Column.CellClass,
                ["fixed"] = item.FixedLeft ? "left" : item.FixedRight ? "right" : null,
                ["left"] = item.Left,
                ["right"] = item.Right,
                ["edgeLeft"] = item.EdgeLeft ? true : null,
                ["edgeRight"] = item.EdgeRight ? true : null
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

/// <summary>列布局信息：固定方向、渲染宽度与吸附偏移量（px）。</summary>
public sealed class TableColumnLayout
{
    /// <summary>列定义。</summary>
    public TaColumnModel Column { get; set; } = new();

    /// <summary>是否吸附左侧。</summary>
    public bool FixedLeft { get; set; }

    /// <summary>是否吸附右侧。</summary>
    public bool FixedRight { get; set; }

    /// <summary>渲染宽度（固定列必填，非固定列可空）。</summary>
    public int? Width { get; set; }

    /// <summary>左侧偏移量。</summary>
    public int? Left { get; set; }

    /// <summary>右侧偏移量。</summary>
    public int? Right { get; set; }

    /// <summary>是否为最靠中间的左固定列（承载右向滚动阴影）。</summary>
    public bool EdgeLeft { get; set; }

    /// <summary>是否为最靠中间的右固定列（承载左向滚动阴影）。</summary>
    public bool EdgeRight { get; set; }
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
