#Requires -Version 7.0
<#
.SYNOPSIS
  打包 RCL 与 dotnet new 模板包到 artifacts 目录。
.DESCRIPTION
  打包前强制校验前端产物存在且非空，避免发布「无样式」的包。
#>
param(
    [string]$Version = '1.1.2'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$artifacts = Join-Path $root 'artifacts'
$css = Join-Path $root 'src/TailAdmin.Templates.Rcl/wwwroot/css/tailadmin.css'
$js = Join-Path $root 'src/TailAdmin.Templates.Rcl/wwwroot/js/tailadmin.js'

Write-Host "[1/4] 校验前端产物 ..." -ForegroundColor Cyan
foreach ($file in @($css, $js)) {
    if (-not (Test-Path $file)) { throw "缺少前端产物：$file（请先执行 build-frontend.ps1）" }
    if ((Get-Item $file).Length -lt 20KB) { throw "前端产物过小：$file" }
}

Write-Host "[2/4] 恢复依赖 ..." -ForegroundColor Cyan
dotnet restore (Join-Path $root 'src/TailAdmin.Templates.Rcl/TailAdmin.Templates.Rcl.csproj')
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore 失败' }

Write-Host "[3/4] 打包 ..." -ForegroundColor Cyan
New-Item -ItemType Directory -Force -Path $artifacts | Out-Null
dotnet pack (Join-Path $root 'src/TailAdmin.Templates.Rcl/TailAdmin.Templates.Rcl.csproj') -c Release -o $artifacts "-p:Version=$Version"
if ($LASTEXITCODE -ne 0) { throw 'RCL 打包失败' }

# 模板内容中的 RCL 版本占位符必须与本次包版本一致，先复制到 staging 再替换，保持源文件可读
$packageRoot = Join-Path $root 'src/TailAdmin.Templates.Package'
$staging = Join-Path $packageRoot 'obj/template-staging'
if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
Copy-Item (Join-Path $packageRoot 'templates') $staging -Recurse
Get-ChildItem $staging -Recurse -File | ForEach-Object {
    $text = Get-Content -Raw -Encoding utf8 $_.FullName
    if ($text.Contains('__TAILADMIN_RCL_VERSION__')) {
        $text = $text.Replace('__TAILADMIN_RCL_VERSION__', $Version)
        Set-Content -Path $_.FullName -Value $text -Encoding utf8 -NoNewline
    }
}

dotnet pack (Join-Path $packageRoot 'TailAdmin.Templates.Package.csproj') -c Release -o $artifacts "-p:Version=$Version" '-p:TemplateSourceRoot=obj/template-staging'
if ($LASTEXITCODE -ne 0) { throw '模板包打包失败' }

Write-Host "[4/4] 输出清单 ..." -ForegroundColor Cyan
Get-ChildItem $artifacts -Filter *.nupkg | ForEach-Object {
    Write-Host ("  {0}  {1:0.0} KB" -f $_.Name, ($_.Length / 1KB)) -ForegroundColor Green
}
