/**
 * Ta.format：金额、日期、布尔等展示格式化。
 * 页面可通过 Ta.format.xxx 扩展自定义列格式化函数（taTable 的 formatter 字段按名称查找）。
 */
export function installFormat(Ta) {
  const format = {
    money(value) {
      if (value === null || value === undefined || value === '') return '';
      const n = Number(value);
      if (Number.isNaN(n)) return String(value);
      return n.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    },
    number(value) {
      if (value === null || value === undefined || value === '') return '';
      const n = Number(value);
      if (Number.isNaN(n)) return String(value);
      return n.toLocaleString('zh-CN');
    },
    date(value) {
      if (!value) return '';
      return String(value).replace('T', ' ').replace(/\.\d+$/, '').slice(0, 19);
    },
    bool(value) {
      return value === true || value === 'true' || value === 1 || value === '1' ? '是' : '否';
    },
    text(value) {
      return value === null || value === undefined ? '' : String(value);
    }
  };
  Ta.format = format;
}
