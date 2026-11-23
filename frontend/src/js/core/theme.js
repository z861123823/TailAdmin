/**
 * 主题控制：与 TailAdmin 官方一致，通过 <html class="dark"> 驱动 dark 变体，
 * localStorage 的 darkMode 记录用户选择。首屏防闪烁由布局 head 内联脚本完成。
 */
export function installTheme(Ta) {
  const storageKey = 'darkMode';
  const root = document.documentElement;

  const systemDark = () =>
    !!(window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches);

  const apply = (dark, persist) => {
    root.classList.toggle('dark', dark);
    root.setAttribute('data-color-scheme', dark ? 'dark' : 'light');
    if (persist) {
      try {
        localStorage.setItem(storageKey, JSON.stringify(!!dark));
      } catch (e) {
        /* 存储不可用时忽略 */
      }
    }
    document.dispatchEvent(
      new CustomEvent('ta:theme-changed', { detail: { theme: dark ? 'dark' : 'light' } })
    );
    return dark ? 'dark' : 'light';
  };

  const current = () => (root.classList.contains('dark') ? 'dark' : 'light');

  Ta.theme = {
    current,
    apply: (mode, persist) => apply(mode === 'dark' || mode === true, persist),
    set: (mode) => apply(mode === 'dark' || mode === true, true),
    toggle: () => {
      const next = !root.classList.contains('dark');
      apply(next, true);
      return next ? 'dark' : 'light';
    },
    init() {
      let stored = null;
      try {
        stored = localStorage.getItem(storageKey);
      } catch (e) {
        stored = null;
      }

      let dark;
      if (stored !== null) {
        try {
          dark = JSON.parse(stored) === true;
        } catch (e) {
          dark = false;
        }
      } else {
        const preset = ((Ta.config && Ta.config.defaultTheme) || 'system').toLowerCase();
        dark = preset === 'dark' || (preset !== 'light' && systemDark());
      }
      apply(dark, false);

      if (window.matchMedia) {
        const mq = window.matchMedia('(prefers-color-scheme: dark)');
        const handler = () => {
          let saved = null;
          try {
            saved = localStorage.getItem(storageKey);
          } catch (e) {
            saved = null;
          }
          if (saved === null) apply(mq.matches, false);
        };
        if (mq.addEventListener) mq.addEventListener('change', handler);
        else if (mq.addListener) mq.addListener(handler);
      }
    }
  };
}
