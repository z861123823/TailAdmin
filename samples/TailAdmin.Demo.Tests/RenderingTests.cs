using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TailAdmin.Demo.Tests.Infrastructure;

namespace TailAdmin.Demo.Tests;

/// <summary>页面渲染与列表接口冒烟测试。</summary>
public sealed class RenderingTests : IClassFixture<DemoWebFactory>, IClassFixture<AnonymousDemoWebFactory>
{
    private readonly DemoWebFactory _factory;
    private readonly AnonymousDemoWebFactory _anonymousFactory;

    /// <summary>初始化测试。</summary>
    public RenderingTests(DemoWebFactory factory, AnonymousDemoWebFactory anonymousFactory)
    {
        _factory = factory;
        _anonymousFactory = anonymousFactory;
    }

    /// <summary>登录页应使用无侧栏认证布局。</summary>
    [Fact]
    public async Task LoginPage_UsesAuthLayout()
    {
        var client = _anonymousFactory.CreateClient();
        var html = await client.GetStringAsync("/Admin/Account/Login");

        Assert.Contains("ta-auth-card", html);
        Assert.Contains("演示帳號", html);
    }

    /// <summary>未登录访问后台页面应跳转登录页。</summary>
    [Fact]
    public async Task Unauthenticated_AdminPage_RedirectsToLogin()
    {
        var client = _anonymousFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/Goods/Index");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Admin/Account/Login", response.Headers.Location!.ToString());
    }

    /// <summary>已登录仪表盘应渲染侧边栏与统计卡片。</summary>
    [Fact]
    public async Task Dashboard_RendersLayoutAndCards()
    {
        var client = _factory.CreateClient();
        var html = await client.GetStringAsync("/Admin/Home/Index");

        Assert.Contains("ta-sidebar", html);
        Assert.Equal(4, System.Text.RegularExpressions.Regex.Matches(html, "ta-card ta-card-pad").Count);
        Assert.Contains("最近上架", html);
    }

    /// <summary>商品列表页应渲染表格容器与弹窗。</summary>
    [Fact]
    public async Task GoodsIndex_RendersTableAndModal()
    {
        var client = _factory.CreateClient();
        var html = await client.GetStringAsync("/Admin/Goods/Index");

        Assert.Contains("goodsTable", html);
        Assert.Contains("taTable(", html);
        Assert.Contains("goodsEdit", html);
        Assert.Contains("新增商品", html);
    }

    /// <summary>列表接口应保持 { code, msg, count, data } 形状。</summary>
    [Fact]
    public async Task GoodsList_ReturnsLayuiShape()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Admin/Goods/List?page=1&limit=5&sortField=Price&sortOrder=asc");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(0, root.GetProperty("code").GetInt32());
        Assert.Equal(36, root.GetProperty("count").GetInt32());
        Assert.Equal(5, root.GetProperty("data").GetArrayLength());
        Assert.Equal(JsonValueKind.Array, root.GetProperty("data").ValueKind);
    }

    /// <summary>空结果应返回 count=0 与空数组，而不是 null。</summary>
    [Fact]
    public async Task GoodsList_EmptyKeyword_ReturnsEmptyArray()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Admin/Goods/List?keyword=不存在的商品關鍵字");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(0, root.GetProperty("count").GetInt32());
        Assert.Equal(JsonValueKind.Array, root.GetProperty("data").ValueKind);
        Assert.Equal(0, root.GetProperty("data").GetArrayLength());
    }

    /// <summary>编辑弹窗 Partial 应包含表单控件。</summary>
    [Fact]
    public async Task EditForm_ReturnsPartialWithForm()
    {
        var client = _factory.CreateClient();
        var html = await client.GetStringAsync("/Admin/Goods/EditForm?Id=1");

        Assert.Contains("<form", html);
        Assert.Contains("GoodsName", html);
        Assert.Contains("TagSearch", html);
        Assert.Contains("UploadImage", html);
        Assert.DoesNotContain("<!DOCTYPE html>", html);
    }

    /// <summary>缺少防伪令牌的写操作应返回 400。</summary>
    [Fact]
    public async Task Delete_WithoutAntiforgeryToken_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/Admin/Goods/Delete", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Id"] = "1"
        }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
