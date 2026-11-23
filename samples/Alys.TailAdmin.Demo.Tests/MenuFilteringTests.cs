using Microsoft.Extensions.Options;
using Moq;
using Alys.TailAdmin.Rcl.Options;
using Alys.TailAdmin.Rcl.Security;
using Alys.TailAdmin.Rcl.Services;

namespace Alys.TailAdmin.Demo.Tests;

/// <summary>菜单权限过滤测试。</summary>
public sealed class MenuFilteringTests
{
    private static AdminUiOptions BuildOptions()
    {
        return new AdminUiOptions
        {
            Menu = new AdminMenuOptions
            {
                Items = new List<AdminMenuItem>
                {
                    new() { Key = "dashboard", Title = "儀表盤", Controller = "Home", Action = "Index", Permission = "dashboard" },
                    new() { Key = "goods", Title = "商品管理", Controller = "Goods", Action = "Index", Permission = "goods" },
                    new()
                    {
                        Key = "system",
                        Title = "系統設置",
                        Permission = "system",
                        Children = new List<AdminMenuItem>
                        {
                            new() { Key = "users", Title = "用戶", Controller = "Users", Action = "Index", Permission = "system-users" }
                        }
                    }
                }
            }
        };
    }

    /// <summary>只返回当前用户有权限的菜单。</summary>
    [Fact]
    public void GetVisibleMenu_FiltersByPermission()
    {
        var service = new AdminMenuService(Options.Create(BuildOptions()));
        var admin = new Mock<ICurrentAdmin>();
        admin.SetupGet(item => item.Permissions).Returns(new[] { "goods" });

        var menus = service.GetVisibleMenu(admin.Object);

        Assert.Single(menus);
        Assert.Equal("goods", menus[0].Key);
    }

    /// <summary>全部权限通配符应返回完整菜单。</summary>
    [Fact]
    public void GetVisibleMenu_Wildcard_ReturnsAll()
    {
        var service = new AdminMenuService(Options.Create(BuildOptions()));
        var admin = new Mock<ICurrentAdmin>();
        admin.SetupGet(item => item.Permissions).Returns(new[] { "*" });

        var menus = service.GetVisibleMenu(admin.Object);

        Assert.Equal(3, menus.Count);
    }

    /// <summary>无任何权限时应返回空菜单而不是抛异常。</summary>
    [Fact]
    public void GetVisibleMenu_NoPermission_ReturnsEmpty()
    {
        var service = new AdminMenuService(Options.Create(BuildOptions()));
        var admin = new Mock<ICurrentAdmin>();
        admin.SetupGet(item => item.Permissions).Returns(Array.Empty<string>());

        var menus = service.GetVisibleMenu(admin.Object);

        Assert.Empty(menus);
    }

    /// <summary>空菜单配置不应抛异常。</summary>
    [Fact]
    public void GetVisibleMenu_EmptyConfig_ReturnsEmpty()
    {
        var service = new AdminMenuService(Options.Create(new AdminUiOptions()));
        var admin = new Mock<ICurrentAdmin>();
        admin.SetupGet(item => item.Permissions).Returns(new[] { "*" });

        Assert.Empty(service.GetVisibleMenu(admin.Object));
    }

    /// <summary>菜单键存在性校验。</summary>
    [Fact]
    public void ContainsKey_MatchesNestedItems()
    {
        var service = new AdminMenuService(Options.Create(BuildOptions()));

        Assert.True(service.ContainsKey("users"));
        Assert.False(service.ContainsKey("missing"));
    }
}
