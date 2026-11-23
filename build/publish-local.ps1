#Requires -Version 7.0
<#
.SYNOPSIS
  把 artifacts 下的 nupkg 发布到本地文件夹源或内网共享目录。
#>
param(
    [string]$TargetPath = '',
    [switch]$ReinstallTemplate
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$artifacts = Join-Path $root 'artifacts'
if (-not $TargetPath) { $TargetPath = $artifacts }

if (-not (Test-Path $artifacts)) { throw '未找到 artifacts 目录，请先执行 pack-nuget.ps1' }
$packages = Get-ChildItem $artifacts -Filter *.nupkg
if (-not $packages) { throw 'artifacts 目录下没有 nupkg 文件' }

New-Item -ItemType Directory -Force -Path $TargetPath | Out-Null
foreach ($pkg in $packages) {
    Copy-Item $pkg.FullName -Destination $TargetPath -Force
    Write-Host ("已复制 {0} -> {1}" -f $pkg.Name, $TargetPath) -ForegroundColor Green
}

if ($ReinstallTemplate) {
    dotnet new uninstall TailAdmin.Templates 2>$null | Out-Null
    dotnet new install TailAdmin.Templates --add-source $TargetPath
    if ($LASTEXITCODE -ne 0) { throw '模板安装失败' }
    Write-Host '模板已重新安装：dotnet new tailadmin-admin' -ForegroundColor Green
}
