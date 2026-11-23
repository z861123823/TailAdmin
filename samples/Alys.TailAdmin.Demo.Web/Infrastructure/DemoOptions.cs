namespace Alys.TailAdmin.Demo.Web.Infrastructure;

/// <summary>演示账号配置（仅用于示例宿主，生产项目请接入真实用户体系）。</summary>
public sealed class DemoOptions
{
    /// <summary>演示账号列表。</summary>
    public List<DemoAccount> Accounts { get; set; } = new();
}

/// <summary>演示账号。</summary>
public sealed class DemoAccount
{
    /// <summary>登录名。</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>密码（明文仅用于演示）。</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>显示名。</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>权限码。</summary>
    public List<string> Permissions { get; set; } = new();
}
