// frontend/tailwind.config.js
// 依赖：tailwindcss 3.4.x + daisyui 4.x（版本必须成对锁定）
const brand = require('./theme/brand.json');

/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    '../src/TailAdmin.Templates.Rcl/Views/**/*.cshtml',
    '../src/TailAdmin.Templates.Rcl/Components/**/*.cs',
    '../src/TailAdmin.Templates.Rcl/Models/**/*.cs',
    '../src/TailAdmin.Templates.Rcl/wwwroot/js/**/*.js',
    '../src/TailAdmin.Templates.Package/**/*.cshtml',
    '../samples/**/*.cshtml',
    './src/js/**/*.js'
  ],
  // 明暗切换：<html data-theme="als-dark" class="theme-dark">
  // 用固定标记类，换品牌名时不需要改这里
  darkMode: ['class', '.theme-dark'],
  corePlugins: {
    // 迁移期与 layui 共存时避免全局 reset 冲突，骨架样式在 .ta-app 作用域内自建
    preflight: false
  },
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'PingFang SC', 'Microsoft YaHei', 'system-ui', 'sans-serif']
      },
      boxShadow: {
        card: '0 1px 2px rgba(16, 24, 40, 0.06), 0 1px 3px rgba(16, 24, 40, 0.10)'
      }
    }
  },
  // C# 字符串拼接 / JS 动态生成的类名必须写入 safelist，否则会被裁剪
  safelist: [
    'btn-primary', 'btn-secondary', 'btn-accent', 'btn-ghost', 'btn-outline',
    'btn-error', 'btn-success', 'btn-info', 'btn-warning', 'btn-sm', 'btn-xs',
    'badge-success', 'badge-warning', 'badge-error', 'badge-info', 'badge-ghost',
    'alert-success', 'alert-error', 'alert-info', 'alert-warning',
    'loading', 'loading-spinner', 'loading-sm',
    'skeleton', 'input-error', 'select-error', 'textarea-error',
    'max-w-sm', 'max-w-md', 'max-w-2xl', 'max-w-4xl', 'max-w-full',
    'theme-light', 'theme-dark'
  ],
  plugins: [require('daisyui')],
  daisyui: {
    // 两个主题都从 brand.json 生成，禁止在这里写死色值
    themes: [
      { [brand.name]: brand.light },
      { [`${brand.name}-dark`]: brand.dark }
    ],
    darkTheme: `${brand.name}-dark`,
    logs: false
  }
};
