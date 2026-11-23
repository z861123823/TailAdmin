/**
 * Ta.modal：统一弹窗组件。
 * - 静态弹窗：x-data="taModal({ id, title, size })"
 * - 动态表单：Ta.modal.load(id, url, params) 拉取服务端 Partial 填入弹窗
 */
export function installModal(Ta) {
  const registry = new Map();
  const sizeClass = {
    sm: 'modal-box max-w-sm',
    md: 'modal-box max-w-lg',
    lg: 'modal-box max-w-2xl',
    xl: 'modal-box max-w-4xl',
    full: 'modal-box max-w-full w-full h-full max-h-full rounded-none'
  };

  Ta.modal = {
    register(id, ctx) {
      registry.set(id, ctx);
    },
    open(id, data) {
      const ctx = registry.get(id);
      if (ctx) ctx.open(data);
    },
    close(id) {
      const ctx = registry.get(id);
      if (ctx) ctx.close();
    },
    async load(id, url, params) {
      const ctx = registry.get(id);
      if (!ctx) {
        Ta.toast.error(`彈窗未註冊：${id}`);
        return;
      }
      await ctx.load(url, params);
    },
    async submit(id) {
      const ctx = registry.get(id);
      if (!ctx) {
        Ta.toast.error(`彈窗未註冊：${id}`);
        return;
      }
      await ctx.submit();
    }
  };

  Ta.previewImage = function previewImage(url) {
    if (!url) return;
    const dialog = document.createElement('dialog');
    dialog.className = 'modal';
    const box = document.createElement('div');
    box.className = 'modal-box max-w-4xl bg-base-100';
    const img = document.createElement('img');
    img.className = 'mx-auto max-h-[80vh] w-auto rounded';
    img.alt = '圖片預覽';
    img.src = url;
    const actions = document.createElement('div');
    actions.className = 'modal-action';
    const close = document.createElement('button');
    close.type = 'button';
    close.className = 'btn btn-sm btn-ghost';
    close.textContent = '關閉';
    close.addEventListener('click', () => dialog.close());
    actions.appendChild(close);
    box.appendChild(img);
    box.appendChild(actions);
    dialog.appendChild(box);
    dialog.addEventListener('close', () => dialog.remove());
    dialog.addEventListener('cancel', (e) => e.preventDefault());
    document.body.appendChild(dialog);
    dialog.showModal();
  };

  return function createModal(config) {
    const cfg = config || {};
    return {
      id: cfg.id,
      title: cfg.title || '',
      size: cfg.size || 'md',
      submitting: false,
      current: null,
      get boxClass() {
        return sizeClass[this.size] || sizeClass.md;
      },
      init() {
        registry.set(this.id, this);
        if (cfg.title) this.title = cfg.title;
        this.$el.addEventListener('close', () => {
          if (cfg.destroyOnClose !== false && this.$refs.body) this.$refs.body.innerHTML = '';
        });
      },
      open(data) {
        this.current = data || null;
        this.$el.showModal();
      },
      close() {
        if (this.$el.open) this.$el.close();
      },
      async load(url, params) {
        this.current = params || null;
        try {
          const query = Ta.http.buildQuery(params || {});
          const html = await Ta.http.getText(url + query);
          this.$refs.body.innerHTML = html;
          if (!this.$el.open) this.$el.showModal();
        } catch (e) {
          /* Ta.http.getText 已处理状态码提示 */
        }
      },
      async submit() {
        const body = this.$refs.body;
        if (!body) return;
        const form = body.querySelector('form');
        if (!form) {
          this.close();
          return;
        }
        this.submitting = true;
        try {
          const res = await Ta.http.postForm(form.action, new FormData(form));
          if (Ta.isOk(res)) {
            Ta.toast.success(res.Message || '保存成功');
            this.close();
            document.dispatchEvent(
              new CustomEvent('ta:modal-submitted', { detail: { modalId: this.id, result: res } })
            );
          } else {
            Ta.toast.error(res && res.Message ? res.Message : '保存失敗');
            if (res && res.Errors && Ta.form) Ta.form.applyErrors(form, res.Errors);
          }
        } finally {
          this.submitting = false;
        }
      }
    };
  };
}
