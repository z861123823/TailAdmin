/**
 * x-datepicker：flatpickr 的 Alpine 指令封装。
 * flatpickr 以 vendor 脚本方式按需引入（wwwroot/js/vendor/flatpickr.min.js）。
 */
export function installDatepicker(Alpine) {
  Alpine.directive('datepicker', (el, { expression }, { evaluate, cleanup }) => {
    if (!window.flatpickr) {
      console.warn('[Ta] 未加載 flatpickr，x-datepicker 已跳過');
      return;
    }
    let options = {};
    if (expression) {
      try {
        options = evaluate(expression) || {};
      } catch (e) {
        options = {};
      }
    }
    const defaults = {
      dateFormat: 'Y-m-d',
      allowInput: true,
      disableMobile: true,
      locale: window.flatpickr.l10ns && window.flatpickr.l10ns.zh ? 'zh' : 'default'
    };
    const instance = window.flatpickr(el, Object.assign(defaults, options));
    cleanup(() => instance.destroy());
  });
}
