# Alys.TailAdmin 通用后台模板

基于 **ASP.NET Core MVC（Razor Pages/views） + TailAdmin 官方 HTML 源码（MIT） + Tailwind CSS 4 + Alpine.js** 的后台通用模板，交付三件套：

- `Alys.TailAdmin.Rcl`：RCL 组件库（布局、侧边栏、顶栏、表格、弹窗、上传、按钮/徽章/提示/头像/卡片/轮播等基础 UI 组件）
- `Alys.TailAdmin.Templates`：`dotnet new` 模板包，一条命令生成可运行后台
- `samples/Alys.TailAdmin.Demo.Web`：完整示例宿主（登录、仪表盘、商品列表、分类试点页）

> 详细方案见仓库根目录《后台通用模板建设方案.md》，本文只写实际操作。

## 1. 环境要求

| 用途 | 依赖 |
| --- | --- |
| 运行 / 编译 | .NET SDK 8.0（RCL 同时目标 `net8.0;net9.0`） |
| 前端构建（仅改样式或 JS 时需要） | Node.js 22.x + npm |
| 运行时 | 不依赖 Node、不依赖 CDN，静态资源随包分发 |

## 2. 快速开始

```powershell
# 1) 前端产物已入库，直接编译即可
dotnet build Alys.TailAdmin.sln

# 2) 运行示例宿主
dotnet run --project samples/Alys.TailAdmin.Demo.Web --launch-profile http
# 打开 http://localhost:5188/Admin/Account/Login
# admin / 123456（全部菜单）
# operator / 123456（仅仪表盘与商品管理，演示权限过滤）
```

修改主题或前端组件后重新生成产物：

```powershell
./build/build-frontend.ps1
```

## 3. 测试与打包

```powershell
dotnet test samples/Alys.TailAdmin.Demo.Tests
./build/pack-nuget.ps1          # 输出到 artifacts/
./build/publish-local.ps1 -ReinstallTemplate   # 复制 nupkg 并安装 dotnet new 模板
```

## 4. 用模板新建项目

```powershell
dotnet new install ./artifacts/Alys.TailAdmin.Templates.1.5.0.nupkg
dotnet new alys-admin -n MyCompany.Admin -o ../MyCompany.Admin

# 可选参数
dotnet new alys-admin -n MyCompany.Admin --auth none --sample false --theme als
```

| 参数 | 取值 | 说明 |
| --- | --- | --- |
| `--auth` | `cookie`（默认）/ `none` | 是否生成 Cookie 登录、登出与登录页 |
| `--sample` | `true`（默认）/ `false` | 是否生成商品列表示例页与演示菜单 |
| `--theme` | 默认 `als` | 品牌主题名，需与 `frontend/theme/brand.json` 的 `name` 一致 |

> 建议把新项目生成到本仓库之外的目录，避免继承本仓库的 `Directory.Packages.props`（中央包管理）。

## 5. 目录结构

```text
.
├─ Directory.Build.props / Directory.Packages.props / nuget.config
├─ Alys.TailAdmin.sln
├─ build/                      # 前端构建、打包、本地发布脚本
├─ frontend/                   # Tailwind 4 + Alpine 源码（构建期使用，运行时不依赖 Node）
├─ src/
│  ├─ Alys.TailAdmin.Rcl/ # 组件库（Views / TagHelpers / 静态资源）
│  └─ Alys.TailAdmin.Templates.Package/  # dotnet new 模板包
├─ samples/
│  ├─ Alys.TailAdmin.Demo.Web/      # 示例宿主
│  └─ Alys.TailAdmin.Demo.Tests/    # xUnit 测试
├─ docs/                       # 组件、主题、接口、迁移记录
└─ artifacts/                  # nupkg 输出（不入库）
```

## 6. 文档

- [组件使用说明](docs/组件使用说明.md)
- [主题与换品牌指引](docs/主题与换品牌指引.md)
- [后端接口约定](docs/后端接口约定.md)
- [官方界面基线重构记录](docs/迁移记录/官方界面基线重构.md)
- [第三方许可说明](docs/第三方许可说明.md)
- [迁移记录](docs/迁移记录/v1.0.0.md)

## 7. 常见问题

- **页面没有样式 / 静态资源 404**：开发环境请设置 `ASPNETCORE_ENVIRONMENT=Development`（示例已带 launchSettings）；生产环境静态资源由 RCL 的 staticwebassets 随包发布，无需复制文件。
- **启动报「菜单键未配置」**：`ViewData["MenuKey"]` 必须是 `AdminUi:Menu` 中存在的 `Key`；开发环境会直接抛异常，生产环境记录警告。
- **修改主题色不生效**：颜色只允许改 `frontend/theme/brand.json`，然后执行 `build/build-frontend.ps1` 重建 `tailadmin.css`。
- **列表接口返回 401 而不是 JSON**：登录已过期，`Ta.http` 会自动跳转登录页；请确认 `AdminUi:LoginPath` 配置正确。
