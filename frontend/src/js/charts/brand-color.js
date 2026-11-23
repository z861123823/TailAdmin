/** 读取构建期生成的 TailAdmin brand 调色板，确保图表跟随品牌色。 */
export function brandColor(step = '500', fallback = '#465fff') {
  const value = getComputedStyle(document.documentElement)
    .getPropertyValue(`--color-brand-${step}`)
    .trim();
  return value || fallback;
}
