/**
 * Ta.toast：轻量提示，渲染到布局内的 #taToastHost。
 * 只使用 textContent 写入业务文案，避免 XSS。
 */
export function installToast(Ta) {
  const variantClass = {
    success: 'alert-success',
    error: 'alert-error',
    info: 'alert-info',
    warning: 'alert-warning'
  };

  const show = (message, type, options) => {
    const opts = options || {};
    const host = document.getElementById('taToastHost');
    if (!host) return null;

    const item = document.createElement('div');
    item.className = `alert ${variantClass[type] || variantClass.info} pointer-events-auto shadow-card`;
    item.setAttribute('role', 'status');

    const text = document.createElement('span');
    text.className = 'text-sm';
    text.textContent = message == null ? '' : String(message);
    item.appendChild(text);

    const close = document.createElement('button');
    close.type = 'button';
    close.className = 'btn btn-ghost btn-xs';
    close.textContent = '关闭';
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
