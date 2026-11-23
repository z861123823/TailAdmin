using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Alys.TailAdmin.Rcl.Options;
using Alys.TailAdmin.Rcl.Security;
using Alys.TailAdmin.Rcl.Services;

namespace Alys.TailAdmin.Rcl.Extensions;

/// <summary>
/// RCL 服务注册入口。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册 Alys.TailAdmin 后台界面所需服务：配置绑定、菜单服务与默认当前用户实现。
    /// 宿主应在调用本方法后注册自己的 <see cref="ICurrentAdmin"/> 实现。
    /// </summary>
    /// <param name="services">服务容器。</param>
    /// <param name="configuration">应用配置。</param>
    /// <returns>服务容器，便于链式调用。</returns>
    public static IServiceCollection AddAlysAdminUi(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<AdminUiOptions>()
            .Bind(configuration.GetSection(AdminUiOptions.SectionName));

        services.TryAddSingleton<ICurrentAdmin, AllowAllCurrentAdmin>();
        services.AddScoped<AdminMenuService>();

        return services;
    }

    /// <summary>
    /// 使用代码方式配置后台界面。
    /// </summary>
    /// <param name="services">服务容器。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>服务容器，便于链式调用。</returns>
    public static IServiceCollection AddAlysAdminUi(this IServiceCollection services, Action<AdminUiOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<AdminUiOptions>().Configure(configure);
        services.TryAddSingleton<ICurrentAdmin, AllowAllCurrentAdmin>();
        services.AddScoped<AdminMenuService>();

        return services;
    }
}
