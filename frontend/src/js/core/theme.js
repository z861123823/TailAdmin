/**
 * 主题控制：data-theme 驱动 daisyUI 主题变量，theme-dark 类驱动 Tailwind dark 变体。
 * 首屏防闪烁由布局 head 内的内联脚本完成，这里只负责切换与持久化。
 */
export function installTheme(Ta) {
  const brand = (Ta.config && Ta.config.brandName) || 'als';
  const storageKey = 'ta-theme';
  const root = document.documentElement;

  const systemTheme = () => {
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) return 'dark';
    return 'light';
  };

  const resolve = (mode) => (mode === 'light' || mode === 'dark' ? mode : systemTheme());

  const apply = (mode, persist) => {
    const resolved = resolve(mode);
    root.setAttribute('data-theme', resolved === 'dark' ? `${brand}-dark` : brand);
    root.classList.toggle('theme-dark', resolved === 'dark');
    root.classList.toggle('theme-light', resolved !== 'dark');
    if (persist) {
      try {
        localStorage.setItem(storageKey, resolved);
      } catch (e) {
        /* 存储不可用时忽略 */
      }
    }
    document.dispatchEvent(new CustomEvent('ta:theme-changed', { detail: { theme: resolved } }));
    return resolved;
  };

  const current = () => (root.classList.contains('theme-dark') ? 'dark' : 'light');

  Ta.theme = {
    current,
    apply,
    set: (mode) => apply(mode, true),
    toggle: () => {
      const next = current() === 'dark' ? 'light' : 'dark';
      apply(next, true);
      return next;
    },
    init() {
      let stored = null;
      try {
        stored = localStorage.getItem(storageKey);
      } catch (e) {
        stored = null;
      }
      apply(stored || (Ta.config && Ta.config.defaultTheme) || 'system', false);

      if (window.matchMedia) {
        const mq = window.matchMedia('(prefers-color-scheme: dark)');
        const handler = () => {
          let mode = null;
          try {
            mode = localStorage.getItem(storageKey);
          } catch (e) {
            mode = null;
          }
          if (!mode) apply(systemTheme(), false);
        };
        if (mq.addEventListener) mq.addEventListener('change', handler);
        else if (mq.addListener) mq.addListener(handler);
      }
    }
  };
}
