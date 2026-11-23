/**
 * Ta.modal：统一弹窗组件。
 * - 静态弹窗：x-data="taModal({ id, title, size })"
 * - 动态表单：Ta.modal.load(id, url, params) 拉取服务端 Partial 填入弹窗
 */
export function installModal(Ta) {
  const registry = new Map();
  const sizeClass = {
    sm: 'max-w-md',
    md: 'max-w-lg',
    lg: 'max-w-2xl',
    xl: 'max-w-4xl',
    full: 'h-full max-w-none rounded-none'
  };
  const outlineButton =
    'inline-flex items-center justify-center rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 shadow-theme-xs transition-all outline-none select-none hover:bg-gray-50 active:translate-y-px dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/3';

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
    dialog.className = 'ta-dialog';
    const box = document.createElement('div');
    box.className = 'ta-dialog-card max-w-4xl';
    const img = document.createElement('img');
    img.className = 'mx-auto max-h-[80vh] w-auto rounded-xl';
    img.alt = '圖片預覽';
    img.src = url;
    const actions = document.createElement('div');
    actions.className = 'mt-5 flex justify-end';
    const close = document.createElement('button');
    close.type = 'button';
    close.className = outlineButton;
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
      // 取弹窗内容容器：优先 $refs，兜底按属性直接查询，避免 ref 丢失后无法再次打开
      bodyEl() {
        return this.$refs.body || this.$root.querySelector('[x-ref="body"]');
      },
      init() {
        registry.set(this.id, this);
        if (cfg.title) this.title = cfg.title;
        // 注意：$el 是指令所在元素，按钮触发时是按钮本身；组件根节点必须用 $root
        this.$root.addEventListener('close', () => {
          if (cfg.destroyOnClose === false) return;
          const body = this.bodyEl();
          if (!body) return;
          // 只销毁注入的子节点，绝不对 body 自身调用 destroyTree：
          // 否则会触发 body 上 x-ref 的清理回调，把 dialog._x_refs.body 删除，导致再次打开时 $refs.body 丢失。
          if (Ta.alpine) {
            Array.from(body.children).forEach((child) => Ta.alpine.destroyTree(child));
          }
          body.innerHTML = '';
        });
      },
      open(data) {
        this.current = data || null;
        this.$root.showModal();
      },
      close() {
        if (this.$root.open) this.$root.close();
      },
      async load(url, params) {
        this.current = params || null;
        try {
          const query = Ta.http.buildQuery(params || {});
          const html = await Ta.http.getText(url + query);
          const body = this.bodyEl();
          if (!body) return;
          body.innerHTML = html;
          // 动态注入的 Partial 含 Alpine 组件（上传、标签等），需手动初始化才会生效
          if (Ta.alpine) Ta.alpine.initTree(body);
          if (!this.$root.open) this.$root.showModal();
        } catch (e) {
          /* Ta.http.getText 已处理状态码提示 */
        }
      },
      async submit() {
        const body = this.bodyEl();
        if (!body || this.submitting) return;
        const form = body.querySelector('form');
        if (!form) {
          this.close();
          return;
        }
        this.submitting = true;
        try {
          const res = await Ta.http.postForm(form.action, new FormData(form));
          if (Ta.isOk(res)) {
            Ta.toast.success(Ta.value(res, 'Message') || '保存成功');
            this.close();
            document.dispatchEvent(
              new CustomEvent('ta:modal-submitted', { detail: { modalId: this.id, result: res } })
            );
          } else {
            // 失败分支保留弹窗，仅提示错误并回填字段级错误
            Ta.toast.error(Ta.value(res, 'Message') || '保存失敗');
            const errors = Ta.value(res, 'Errors');
            if (errors && Ta.form) Ta.form.applyErrors(form, errors);
          }
        } catch (e) {
          /* Ta.http 已统一弹出状态码/网络异常提示，此处阻止未捕获异常中断交互 */
        } finally {
          this.submitting = false;
        }
      }
    };
  };
}
