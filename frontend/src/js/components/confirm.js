/**
 * Ta.confirm：基于原生 <dialog> 的二次确认，视觉沿用 TailAdmin 官方弹窗样式，返回 Promise<boolean>。
 */
export function installConfirm(Ta) {
  const createButton = (className, text) => {
    const btn = document.createElement('button');
    btn.type = 'button';
    btn.className = className;
    btn.textContent = text;
    return btn;
  };

  const outlineButton =
    'inline-flex items-center justify-center rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 shadow-theme-xs transition-all outline-none select-none hover:bg-gray-50 active:translate-y-px dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/3';
  const primaryButton =
    'inline-flex items-center justify-center rounded-lg bg-brand-500 px-4 py-2.5 text-sm font-medium text-white shadow-theme-xs transition-all outline-none select-none hover:bg-brand-600 active:translate-y-px';
  const dangerButton =
    'inline-flex items-center justify-center rounded-lg bg-red-500 px-4 py-2.5 text-sm font-medium text-white shadow-theme-xs transition-all outline-none select-none hover:bg-red-600 active:translate-y-px';

  Ta.confirm = function confirm(options) {
    const opts = options || {};
    return new Promise((resolve) => {
      const dialog = document.createElement('dialog');
      dialog.className = 'ta-dialog';

      const box = document.createElement('div');
      box.className = 'ta-dialog-card max-w-md';

      const title = document.createElement('h3');
      title.className = 'text-lg font-semibold text-gray-800 dark:text-white/90';
      title.textContent = opts.title || '操作確認';

      const text = document.createElement('p');
      text.className = 'mt-2 text-sm leading-6 text-gray-500 dark:text-gray-400';
      text.textContent = opts.text || '';

      const actions = document.createElement('div');
      actions.className = 'mt-6 flex flex-col-reverse justify-end gap-3 sm:flex-row';

      const cancel = createButton(outlineButton, opts.cancelText || '取消');
      const ok = createButton(opts.danger ? dangerButton : primaryButton, opts.confirmText || '確定');

      let result = false;
      const finish = (value) => {
        result = value;
        dialog.close();
      };
      cancel.addEventListener('click', () => finish(false));
      ok.addEventListener('click', () => finish(true));

      dialog.addEventListener('close', () => {
        dialog.remove();
        resolve(result);
      });
      dialog.addEventListener('cancel', (e) => {
        e.preventDefault();
        finish(false);
      });

      actions.appendChild(cancel);
      actions.appendChild(ok);
      box.appendChild(title);
      box.appendChild(text);
      box.appendChild(actions);
      dialog.appendChild(box);
      document.body.appendChild(dialog);
      dialog.showModal();
      ok.focus();
    });
  };
}
