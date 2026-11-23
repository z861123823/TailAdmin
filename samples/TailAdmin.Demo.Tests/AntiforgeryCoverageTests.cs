using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using TailAdmin.Demo.Web.Areas.Admin.Controllers;

namespace TailAdmin.Demo.Tests;

/// <summary>防伪令牌覆盖测试：防止新增 POST 动作漏加校验特性。</summary>
public sealed class AntiforgeryCoverageTests
{
    /// <summary>所有非 GET 动作必须由控制器级 AutoValidateAntiforgeryToken 覆盖，或显式声明豁免。</summary>
    [Fact]
    public void AllWriteActions_AreCoveredByAntiforgeryValidation()
    {
        var controllerTypes = typeof(GoodsController).Assembly
            .GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract && !type.IsGenericTypeDefinition)
            .ToList();

        Assert.NotEmpty(controllerTypes);
        var uncovered = new List<string>();

        foreach (var controller in controllerTypes)
        {
            var controllerCovered = controller.IsDefined(typeof(AutoValidateAntiforgeryTokenAttribute), inherit: true);
            var writeActions = controller
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => IsWriteAction(method));

            foreach (var action in writeActions)
            {
                var validated = controllerCovered
                    || action.IsDefined(typeof(AutoValidateAntiforgeryTokenAttribute), inherit: true);
                var ignored = action.IsDefined(typeof(IgnoreAntiforgeryTokenAttribute), inherit: true);

                if (!validated && !ignored)
                {
                    uncovered.Add($"{controller.Name}.{action.Name}");
                }
            }
        }

        Assert.True(uncovered.Count == 0, "以下写操作缺少防伪令牌校验：" + string.Join(", ", uncovered));
    }

    private static bool IsWriteAction(MethodInfo method)
    {
        if (method.IsDefined(typeof(HttpGetAttribute), inherit: true))
        {
            return false;
        }

        if (method.IsDefined(typeof(HttpPostAttribute), inherit: true))
        {
            return true;
        }

        var verbs = method.GetCustomAttributes<HttpMethodAttribute>(inherit: true)
            .SelectMany(attribute => attribute.HttpMethods)
            .ToList();
        return verbs.Any(verb => !string.Equals(verb, "GET", StringComparison.OrdinalIgnoreCase));
    }
}
