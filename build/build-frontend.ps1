#Requires -Version 7.0
<#
.SYNOPSIS
  生成 TailAdmin 前端产物（tailadmin.css / tailadmin.js），并输出到 RCL 的 wwwroot。
.DESCRIPTION
  - 依赖版本由 frontend/package.json 锁定，构建期使用本地 node_modules，运行时不依赖 Node。
  - 若 frontend/tools/esbuild.exe 存在，则优先使用独立可执行文件打包 JS。
  - 产物入库：构建完成后 tailadmin.css / tailadmin.js 可直接提交，保证未安装前端工具链也能 dotnet build。
#>
param(
    [switch]$ForceInstall
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$frontend = Join-Path $root 'frontend'
$rclRoot = Join-Path $root 'src/TailAdmin.Templates.Rcl'
$wwwroot = Join-Path $rclRoot 'wwwroot'
$cssOut = Join-Path $wwwroot 'css/tailadmin.css'
$jsOut = Join-Path $wwwroot 'js/tailadmin.js'

Write-Host "[1/5] 检查前端工具链 ..." -ForegroundColor Cyan
Push-Location $frontend
try {
    if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
        throw '未找到 npm。前端产物需在构建期生成；若在离线环境，请提供已构建好的产物或内网 node 环境。'
    }

    $nodeModules = Join-Path $frontend 'node_modules'
    if ($ForceInstall -or -not (Test-Path $nodeModules)) {
        Write-Host '  npm install ...' -ForegroundColor DarkGray
        npm install --no-audit --no-fund
        if ($LASTEXITCODE -ne 0) { throw 'npm install 失败' }
    }

    Write-Host "[2/5] 生成 CSS（Tailwind 3.4 + daisyUI 4，品牌主题来自 theme/brand.json）..." -ForegroundColor Cyan
    npm run build:css
    if ($LASTEXITCODE -ne 0) { throw 'tailwind 构建失败' }

    Write-Host "[3/5] 生成 JS（Alpine + TanStack Table Core + 组件）..." -ForegroundColor Cyan
    $esbuildExe = Join-Path $frontend 'tools/esbuild.exe'
    if (Test-Path $esbuildExe) {
        Write-Host '  使用 frontend/tools/esbuild.exe' -ForegroundColor DarkGray
        & $esbuildExe 'src/js/index.js' --bundle --minify --format=iife --target=es2019 --legal-comments=none "--outfile=$jsOut"
        if ($LASTEXITCODE -ne 0) { throw 'esbuild 构建失败' }
    }
    else {
        npm run build:js
        if ($LASTEXITCODE -ne 0) { throw 'esbuild 构建失败' }
    }

    Write-Host "[4/5] 拷贝第三方 vendor 资源（flatpickr）..." -ForegroundColor Cyan
    $vendorJs = Join-Path $wwwroot 'js/vendor'
    $vendorCss = Join-Path $wwwroot 'css/vendor'
    New-Item -ItemType Directory -Force -Path $vendorJs, $vendorCss | Out-Null
    Copy-Item (Join-Path $frontend 'node_modules/flatpickr/dist/flatpickr.min.js') (Join-Path $vendorJs 'flatpickr.min.js') -Force
    Copy-Item (Join-Path $frontend 'node_modules/flatpickr/dist/l10n/zh.js') (Join-Path $vendorJs 'flatpickr.zh.js') -Force
    Copy-Item (Join-Path $frontend 'node_modules/flatpickr/dist/flatpickr.min.css') (Join-Path $vendorCss 'flatpickr.min.css') -Force

    Write-Host "[5/5] 校验产物 ..." -ForegroundColor Cyan
    foreach ($file in @($cssOut, $jsOut)) {
        if (-not (Test-Path $file)) { throw "产物缺失：$file" }
        if ((Get-Item $file).Length -lt 20KB) { throw "产物过小，疑似构建异常：$file" }
    }
    $cssText = Get-Content -Raw -Encoding utf8 $cssOut
    foreach ($needle in @('.btn', 'data-theme', '--p')) {
        if (-not $cssText.Contains($needle)) { throw "CSS 产物缺少关键标记：$needle" }
    }
    $jsText = Get-Content -Raw -Encoding utf8 $jsOut
    foreach ($needle in @('taTable', 'Ta')) {
        if (-not $jsText.Contains($needle)) { throw "JS 产物缺少关键标记：$needle" }
    }
    $cssSize = [math]::Round((Get-Item $cssOut).Length / 1KB, 1)
    $jsSize = [math]::Round((Get-Item $jsOut).Length / 1KB, 1)
    Write-Host "构建完成：tailadmin.css=${cssSize}KB, tailadmin.js=${jsSize}KB" -ForegroundColor Green
}
finally {
    Pop-Location
}
