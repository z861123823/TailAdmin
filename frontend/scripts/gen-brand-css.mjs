// 由 theme/brand.json 生成 TailAdmin 官方 brand-50 ~ brand-950 调色板。
// TailAdmin 官方模板的品牌色直接写在 CSS 变量里，这里改为构建期生成，
// 保留品牌可配置能力，同时不改变官方组件外观。
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const here = path.dirname(fileURLToPath(import.meta.url));
const brandPath = path.resolve(here, "../theme/brand.json");
const outPath = path.resolve(here, "../src/css/brand.generated.css");
const brand = JSON.parse(fs.readFileSync(brandPath, "utf8"));

const normalizeHex = (value, fallback) => {
  const text = String(value || "").trim();
  if (/^#[0-9a-f]{6}$/i.test(text)) return text.toLowerCase();
  if (/^#[0-9a-f]{3}$/i.test(text)) {
    return (
      "#" +
      text
        .slice(1)
        .split("")
        .map((c) => c + c)
        .join("")
    ).toLowerCase();
  }
  return fallback;
};

const toRgb = (hex) => ({
  r: parseInt(hex.slice(1, 3), 16),
  g: parseInt(hex.slice(3, 5), 16),
  b: parseInt(hex.slice(5, 7), 16),
});

const toHex = (rgb) =>
  "#" +
  [rgb.r, rgb.g, rgb.b]
    .map((v) => Math.max(0, Math.min(255, Math.round(v))).toString(16).padStart(2, "0"))
    .join("");

const mix = (base, target, weight) => {
  const a = toRgb(base);
  const b = toRgb(target);
  return toHex({
    r: a.r + (b.r - a.r) * weight,
    g: a.g + (b.g - a.g) * weight,
    b: a.b + (b.b - a.b) * weight,
  });
};

const steps = [
  [50, "#ffffff", 0.94],
  [100, "#ffffff", 0.88],
  [200, "#ffffff", 0.75],
  [300, "#ffffff", 0.58],
  [400, "#ffffff", 0.32],
  [500, null, 0],
  [600, "#000000", 0.1],
  [700, "#000000", 0.22],
  [800, "#000000", 0.34],
  [900, "#000000", 0.46],
  [950, "#000000", 0.62],
];

const buildPalette = (primary, fallback) => {
  const base = normalizeHex(primary, fallback);
  const palette = {};
  for (const [step, target, weight] of steps) {
    palette[step] = target ? mix(base, target, weight) : base;
  }
  return palette;
};

const light = buildPalette(brand.light && brand.light.primary, "#465fff");
const dark = buildPalette(brand.dark && brand.dark.primary, light[500]);

const block = (palette, indent) =>
  Object.keys(palette)
    .map((step) => `${indent}--color-brand-${step}: ${palette[step]};`)
    .join("\n");

const css = `/* 自动生成，请修改 frontend/theme/brand.json 后重新执行 npm run build:brand */
@theme {
${block(light, "  ")}
}

@layer base {
  .dark {
${block(dark, "    ")}
  }
}
`;

fs.writeFileSync(outPath, css, "utf8");
console.log(`brand.generated.css 已生成：light=${light[500]}, dark=${dark[500]}`);
