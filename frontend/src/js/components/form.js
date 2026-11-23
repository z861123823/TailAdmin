/**
 * taForm + Ta.form.applyErrors：
 * 前端只做即时提示，字段级错误以服务端返回的 Errors 为准统一回填。
 */
const ERROR_CLASSES = [
  'border-red-500',
  'ring-3',
  'ring-red-500/20',
  'focus:border-red-500',
  'focus:ring-red-500/20',
  'dark:border-red-500/50',
  'dark:ring-red-500/40'
];

export function installForm(Ta) {
  const applyErrors = (form, errors) => {
    if (!form || !errors) return;
    form.querySelectorAll('[data-valmsg-for]').forEach((el) => {
      el.textContent = '';
    });
    form.querySelectorAll('[aria-invalid="true"]').forEach((el) => {
      el.removeAttribute('aria-invalid');
      el.classList.remove(...ERROR_CLASSES);
    });

    Object.keys(errors).forEach((key) => {
      const messages = errors[key];
      const text = Array.isArray(messages) ? messages.join('；') : String(messages || '');
      const msg = form.querySelector(`[data-valmsg-for="${key}"]`);
      if (msg) msg.textContent = text;

      const field = form.querySelector(`[name="${key}"], [name="${key}[]"]`);
      if (field) {
        field.setAttribute('aria-invalid', 'true');
        field.classList.add(...ERROR_CLASSES);
      }
    });
  };

  const clearErrors = (form) => applyErrors(form, {});

  Ta.form = { applyErrors, clearErrors };

  return function createForm() {
    return {
      submitting: false,
      errors: {},
      async submit(el) {
        const form = el || this.$el;
        if (!form) return;
        this.submitting = true;
        clearErrors(form);
        try {
          const res = await Ta.http.postForm(form.action, new FormData(form));
          if (Ta.isOk(res)) {
            Ta.toast.success(res.Message || '保存成功');
            const url = form.dataset.successUrl;
            if (url) {
              window.location.href = url;
              return;
            }
            document.dispatchEvent(new CustomEvent('ta:form-saved', { detail: { form, result: res } }));
          } else {
            Ta.toast.error(res && res.Message ? res.Message : '保存失敗');
            if (res && res.Errors) applyErrors(form, res.Errors);
          }
        } finally {
          this.submitting = false;
        }
      }
    };
  };
}
