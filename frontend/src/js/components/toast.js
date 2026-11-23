/**
 * Ta.toast：轻量提示，渲染到布局内的 #taToastHost。
 * 外观沿用 TailAdmin 官方 Alert 样式，只使用 textContent 写入业务文案，避免 XSS。
 */
export function installToast(Ta) {
  const variantClass = {
    success:
      'border-green-500/60 bg-green-50 text-gray-700 dark:border-green-500/30 dark:bg-green-500/10 dark:text-gray-300',
    error:
      'border-red-500/60 bg-red-50 text-gray-700 dark:border-red-500/30 dark:bg-red-500/10 dark:text-gray-300',
    info:
      'border-blue-500/60 bg-blue-50 text-gray-700 dark:border-blue-500/30 dark:bg-blue-500/10 dark:text-gray-300',
    warning:
      'border-amber-500/60 bg-amber-50 text-gray-700 dark:border-amber-500/30 dark:bg-amber-500/10 dark:text-gray-300'
  };

  const show = (message, type, options) => {
    const opts = options || {};
    const host = document.getElementById('taToastHost');
    if (!host) return null;

    const item = document.createElement('div');
    item.className = `pointer-events-auto flex items-start gap-3 rounded-xl border p-4 text-sm shadow-theme-sm ${
      variantClass[type] || variantClass.info
    }`;
    item.setAttribute('role', 'status');

    const text = document.createElement('span');
    text.className = 'min-w-0 flex-1 leading-5';
    text.textContent = message == null ? '' : String(message);
    item.appendChild(text);

    const close = document.createElement('button');
    close.type = 'button';
    close.className =
      'ml-auto shrink-0 text-gray-400 transition-colors hover:text-gray-700 dark:text-gray-500 dark:hover:text-white';
    close.setAttribute('aria-label', '关闭');
    close.textContent = '×';
    close.addEventListener('click', () => item.remove());
    item.appendChild(close);

    host.appendChild(item);

    const duration = opts.duration === undefined ? 2500 : opts.duration;
    if (duration > 0) {
      window.setTimeout(() => {
        if (item.parentNode) item.remove();
      }, duration);
    }
    return item;
  };

  Ta.toast = {
    show,
    success: (message, options) => show(message, 'success', options),
    error: (message, options) => show(message, 'error', options),
    info: (message, options) => show(message, 'info', options),
    warning: (message, options) => show(message, 'warning', options)
  };
}
