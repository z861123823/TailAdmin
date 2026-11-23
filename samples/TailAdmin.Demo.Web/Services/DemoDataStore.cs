using System.Globalization;
using TailAdmin.Demo.Web.Models;

namespace TailAdmin.Demo.Web.Services;

/// <summary>
/// 示例宿主的内存数据仓储。真实项目请替换为 SqlSugar + SQL Server / MySQL 实现，
/// 本类只用于让模板开箱即可运行演示。
/// </summary>
public sealed class DemoDataStore
{
    private readonly object _sync = new();
    private readonly List<GoodsTypeItem> _types = new();
    private readonly List<GoodsItem> _goods = new();
    private readonly string[] _tags = { "新品", "熱銷", "推薦", "清倉", "限時" };
    private int _goodsId;
    private int _typeId;

    /// <summary>初始化并写入演示数据。</summary>
    public DemoDataStore()
    {
        Seed();
    }

    /// <summary>分页查询商品。</summary>
    public (IReadOnlyList<GoodsListItem> Items, int Total) QueryGoods(
        int page,
        int limit,
        string? sortField,
        string? sortOrder,
        string? keyword,
        int? typeId)
    {
        lock (_sync)
        {
            IEnumerable<GoodsItem> query = _goods;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(item =>
                    item.GoodsName.Contains(kw, StringComparison.OrdinalIgnoreCase)
                    || (item.Remark?.Contains(kw, StringComparison.OrdinalIgnoreCase) ?? false)
                    || item.Tags.Any(tag => tag.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            if (typeId is > 0)
            {
                query = query.Where(item => item.TypeId == typeId.Value);
            }

            query = (sortField, sortOrder?.ToLowerInvariant()) switch
            {
                ("GoodsName", "asc") => query.OrderBy(item => item.GoodsName, StringComparer.Ordinal),
                ("GoodsName", _) => query.OrderByDescending(item => item.GoodsName, StringComparer.Ordinal),
                ("Price", "asc") => query.OrderBy(item => item.Price),
                ("Price", _) => query.OrderByDescending(item => item.Price),
                ("SalesVolume", "asc") => query.OrderBy(item => item.SalesVolume),
                ("SalesVolume", _) => query.OrderByDescending(item => item.SalesVolume),
                ("AddTime", "asc") => query.OrderBy(item => item.AddTime),
                ("AddTime", _) => query.OrderByDescending(item => item.AddTime),
                _ => query.OrderByDescending(item => item.Id)
            };

            var total = query.Count();
            var rows = query.Skip((page - 1) * limit).Take(limit).Select(ToListItem).ToList();
            return (rows, total);
        }
    }

    /// <summary>获取商品详情。</summary>
    public GoodsItem? GetGoods(int id)
    {
        lock (_sync)
        {
            return _goods.FirstOrDefault(item => item.Id == id);
        }
    }

    /// <summary>保存商品。</summary>
    public GoodsItem SaveGoods(GoodsEditModel model)
    {
        lock (_sync)
        {
            var addTime = DateTime.TryParse(model.AddTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
                ? parsed
                : DateTime.Now;

            GoodsItem target;
            if (model.Id > 0)
            {
                target = _goods.FirstOrDefault(item => item.Id == model.Id)
                    ?? throw new InvalidOperationException("商品不存在");
            }
            else
            {
                target = new GoodsItem { Id = ++_goodsId };
                _goods.Add(target);
            }

            target.GoodsName = model.GoodsName.Trim();
            target.PicUrl = string.IsNullOrWhiteSpace(model.PicUrl) ? null : model.PicUrl.Trim();
            target.Price = model.Price;
            target.TypeId = model.TypeId;
            target.Tags = model.Tags.Where(tag => !string.IsNullOrWhiteSpace(tag)).Select(tag => tag.Trim()).Distinct().ToList();
            target.SalesVolume = model.SalesVolume;
            target.IsHot = model.IsHot;
            target.AddTime = addTime;
            target.Remark = string.IsNullOrWhiteSpace(model.Remark) ? null : model.Remark.Trim();
            return target;
        }
    }

    /// <summary>删除商品。</summary>
    public bool DeleteGoods(int id)
    {
        lock (_sync)
        {
            return _goods.RemoveAll(item => item.Id == id) > 0;
        }
    }

    /// <summary>分页查询分类。</summary>
    public (IReadOnlyList<GoodsTypeListItem> Items, int Total) QueryTypes(int page, int limit, string? keyword)
    {
        lock (_sync)
        {
            IEnumerable<GoodsTypeItem> query = _types;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(item => item.Name.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var ordered = query.OrderBy(item => item.Sort).ThenBy(item => item.Id).ToList();
            var total = ordered.Count;
            var rows = ordered
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(item => new GoodsTypeListItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Sort = item.Sort,
                    Enabled = item.Enabled,
                    GoodsCount = _goods.Count(goods => goods.TypeId == item.Id)
                })
                .ToList();
            return (rows, total);
        }
    }

    /// <summary>获取分类详情。</summary>
    public GoodsTypeItem? GetType(int id)
    {
        lock (_sync)
        {
            return _types.FirstOrDefault(item => item.Id == id);
        }
    }

    /// <summary>保存分类。</summary>
    public GoodsTypeItem SaveType(GoodsTypeEditModel model)
    {
        lock (_sync)
        {
            GoodsTypeItem target;
            if (model.Id > 0)
            {
                target = _types.FirstOrDefault(item => item.Id == model.Id)
                    ?? throw new InvalidOperationException("分類不存在");
            }
            else
            {
                target = new GoodsTypeItem { Id = ++_typeId };
                _types.Add(target);
            }

            target.Name = model.Name.Trim();
            target.Sort = model.Sort;
            target.Enabled = model.Enabled;
            return target;
        }
    }

    /// <summary>删除分类；存在商品引用时返回失败原因。</summary>
    public string? DeleteType(int id)
    {
        lock (_sync)
        {
            if (_goods.Any(goods => goods.TypeId == id))
            {
                return "該分類下仍有商品，無法刪除";
            }

            return _types.RemoveAll(item => item.Id == id) > 0 ? null : "分類不存在";
        }
    }

    /// <summary>获取启用中的分类下拉项。</summary>
    public IReadOnlyList<GoodsTypeItem> GetTypes()
    {
        lock (_sync)
        {
            return _types.Where(item => item.Enabled).OrderBy(item => item.Sort).ToList();
        }
    }

    /// <summary>标签远程搜索。</summary>
    public IReadOnlyList<(string Value, string Label)> SearchTags(string? keyword, IEnumerable<string>? ids)
    {
        var wanted = ids?.Where(id => !string.IsNullOrWhiteSpace(id)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (wanted is { Count: > 0 })
        {
            return _tags.Where(tag => wanted.Contains(tag)).Select(tag => (Value: tag, Label: tag)).ToList();
        }

        var query = _tags.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(tag => tag.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.Select(tag => (Value: tag, Label: tag)).ToList();
    }

    /// <summary>仪表盘统计。</summary>
    public DashboardViewModel GetDashboard()
    {
        lock (_sync)
        {
            return new DashboardViewModel
            {
                GoodsCount = _goods.Count,
                SalesAmount = _goods.Sum(item => item.Price * item.SalesVolume),
                TypeCount = _types.Count,
                HotCount = _goods.Count(item => item.IsHot),
                RecentGoods = _goods.OrderByDescending(item => item.Id).Take(8).Select(ToListItem).ToList()
            };
        }
    }

    private GoodsListItem ToListItem(GoodsItem item)
    {
        var typeName = _types.FirstOrDefault(type => type.Id == item.TypeId)?.Name ?? "未分類";
        return new GoodsListItem
        {
            Id = item.Id,
            GoodsName = item.GoodsName,
            PicUrl = item.PicUrl,
            Price = item.Price,
            TypeName = typeName,
            TagsText = string.Join(" / ", item.Tags),
            SalesVolume = item.SalesVolume,
            IsHot = item.IsHot,
            AddTime = item.AddTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        };
    }

    private void Seed()
    {
        var typeNames = new[] { "數碼配件", "家居生活", "運動戶外", "美妝個護", "食品飲料" };
        foreach (var name in typeNames)
        {
            _types.Add(new GoodsTypeItem { Id = ++_typeId, Name = name, Sort = _typeId * 10, Enabled = true });
        }

        var goodsNames = new[]
        {
            "無線藍牙耳機", "機械鍵盤", "人體工學椅", "保溫杯", "瑜伽墊",
            "香薰蠟燭", "咖啡豆禮盒", "露營帳篷", "智能手環", "護膚套裝",
            "便攜充電寶", "桌面收納盒"
        };

        for (var i = 0; i < 36; i++)
        {
            var baseName = goodsNames[i % goodsNames.Length];
            var batch = i / goodsNames.Length + 1;
            _goods.Add(new GoodsItem
            {
                Id = ++_goodsId,
                GoodsName = $"{baseName} {batch}代",
                Price = Math.Round(29.9m + (i % 9) * 70.15m, 2),
                TypeId = _types[i % _types.Count].Id,
                Tags = new List<string> { _tags[i % _tags.Length], _tags[(i + 2) % _tags.Length] },
                SalesVolume = 12 + i * 7 % 260,
                IsHot = i % 4 == 0,
                AddTime = DateTime.Today.AddDays(-i).AddHours(9).AddMinutes(i % 50),
                Remark = i % 5 == 0 ? "演示數據，可直接編輯或刪除" : null
            });
        }
    }
}
