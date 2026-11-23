/**
 * Ta.confirm：基于原生 <dialog> + daisyUI modal 的二次确认，返回 Promise<boolean>。
 */
export function installConfirm(Ta) {
  const createButton = (className, text) => {
    const btn = document.createElement('button');
    btn.type = 'button';
    btn.className = className;
    btn.textContent = text;
    return btn;
  };

  Ta.confirm = function confirm(options) {
    const opts = options || {};
    return new Promise((resolve) => {
      const dialog = document.createElement('dialog');
      dialog.className = 'modal';

      const box = document.createElement('div');
      box.className = 'modal-box max-w-md';

      const title = document.createElement('h3');
      title.className = 'text-base font-semibold';
      title.textContent = opts.title || '操作確認';

      const text = document.createElement('p');
      text.className = 'py-3 text-sm text-base-content/70';
      text.textContent = opts.text || '';

      const actions = document.createElement('div');
      actions.className = 'modal-action';

      const cancel = createButton('btn btn-ghost btn-sm', opts.cancelText || '取消');
      const ok = createButton(
        `btn btn-sm ${opts.danger ? 'btn-error' : 'btn-primary'}`,
        opts.confirmText || '確定'
      );

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
