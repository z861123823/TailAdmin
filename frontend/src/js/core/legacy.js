/**
 * 兼容层：为旧后台页面保留的全局函数。
 * 新页面禁止调用 escapeHtml / escapeAttr，统一由 Alpine 的 x-text 渲染，从根上避免 XSS。
 */
export function installLegacy(Ta) {
  Ta.isOk = (res) => !!res && (res.Success === true || res.Success === 'true');

  const escapeHtml = (value) => {
    if (value === null || value === undefined) return '';
    return String(value)
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  };
  const escapeAttr = (value) => escapeHtml(value).replace(/`/g, '&#96;');

  Ta.escapeHtml = escapeHtml;
  Ta.escapeAttr = escapeAttr;

  const notify = (res, onSuccess) => {
    const ok = Ta.isOk(res);
    const message = (res && (res.Message || res.msg)) || (ok ? '操作成功' : '操作失敗');
    if (ok) {
      Ta.toast.success(message);
      if (typeof onSuccess === 'function') onSuccess(res);
    } else {
      Ta.toast.error(message);
    }
    return ok;
  };

  // 旧页面兼容入口（新页面请直接使用 Ta.http / Ta.toast）
  window.adminNotify = function (res, onSuccess) {
    return notify(res, onSuccess);
  };
  window.adminPost = function (url, data, done) {
    Ta.http
      .postForm(url, data)
      .then((res) => {
        notify(res);
        if (typeof done === 'function') done(res);
      })
      .catch(() => {});
  };
  window.escapeHtml = escapeHtml;
  window.escapeAttr = escapeAttr;
}
