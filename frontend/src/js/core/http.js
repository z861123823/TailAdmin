/**
 * Ta.http：统一请求封装。
 * - 所有写操作使用 multipart/form-data（由 FormData 构造）
 * - 自动附带防伪令牌（表单隐藏域 + 请求头）
 * - 统一处理 401 / 403 / 400 令牌失效 / 500，并抛出可读错误
 */
export function installHttp(Ta) {
  const tokenValue = () => {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  };

  const makeError = (message, status, body) => {
    const err = new Error(message);
    err.status = status;
    err.body = body;
    return err;
  };

  const toFormData = (data) => {
    if (data instanceof FormData) return data;
    const fd = new FormData();
    if (data && typeof data === 'object') {
      Object.keys(data).forEach((key) => {
        const value = data[key];
        if (value === undefined || value === null) return;
        if (Array.isArray(value)) {
          value.forEach((item) => fd.append(key, item == null ? '' : item));
        } else {
          fd.append(key, value);
        }
      });
    }
    return fd;
  };

  const buildQuery = (params) => {
    if (!params) return '';
    const parts = [];
    Object.keys(params).forEach((key) => {
      const value = params[key];
      if (value === undefined || value === null || value === '') return;
      if (Array.isArray(value)) {
        value.forEach((item) => parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(item)}`));
      } else {
        parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
      }
    });
    return parts.length ? `?${parts.join('&')}` : '';
  };

  const redirectToLogin = () => {
    const login = (Ta.config && Ta.config.loginUrl) || '/Admin/Account/Login';
    const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
    window.location.href = `${login}${login.indexOf('?') >= 0 ? '&' : '?'}returnUrl=${returnUrl}`;
  };

  const handleFailure = (status, body, silent, traceId) => {
    const message = Ta.value(body, 'Message') || (body && body.msg) || '';
    if (status === 401) {
      if (!silent) Ta.toast.error(message || '請重新登入');
      redirectToLogin();
      return makeError(message || '登录已过期', status, body);
    }
    if (status === 400) {
      const text = message || '頁面已過期，請重新整理後重試';
      if (!silent) Ta.toast.error(text);
      return makeError(text, status, body);
    }
    if (status === 403) {
      const text = message || '無權限執行此操作';
      if (!silent) Ta.toast.error(text);
      return makeError(text, status, body);
    }
    if (status === 404) {
      const text = message || '請求的資源不存在';
      if (!silent) Ta.toast.error(text);
      return makeError(text, status, body);
    }
    const text = message || '系統繁忙，請稍後重試';
    if (status >= 500 && traceId) console.error(`[Ta] 请求失败 TraceId=${traceId}`, body);
    if (!silent) Ta.toast.error(text);
    return makeError(text, status, body);
  };

  async function request(url, options = {}, opts = {}) {
    const silent = !!opts.silent;
    const headers = Object.assign(
      { 'X-Requested-With': 'XMLHttpRequest', Accept: 'application/json' },
      options.headers || {}
    );
    const token = tokenValue();
    if (token) headers.RequestVerificationToken = token;

    let response;
    try {
      response = await fetch(url, Object.assign({ credentials: 'same-origin' }, options, { headers }));
    } catch (e) {
      if (!silent) Ta.toast.error('網絡異常，請檢查連接');
      throw makeError('網絡異常', 0, null);
    }

    const text = await response.text();
    let body = null;
    if (text) {
      try {
        body = JSON.parse(text);
      } catch (e) {
        body = null;
      }
    }

    if (!response.ok) {
      const traceId = response.headers.get('X-Trace-Id') || '';
      throw handleFailure(response.status, body, silent, traceId);
    }
    return body;
  }

  const get = (url, params, opts) => request(url + buildQuery(params), { method: 'GET' }, opts);

  const postForm = (url, data, opts) => {
    const fd = toFormData(data);
    const token = tokenValue();
    if (token && !fd.has('__RequestVerificationToken')) fd.append('__RequestVerificationToken', token);
    return request(url, { method: 'POST', body: fd }, opts);
  };

  const upload = (url, formData, opts = {}) =>
    new Promise((resolve, reject) => {
      const fd = toFormData(formData);
      const token = tokenValue();
      if (token && !fd.has('__RequestVerificationToken')) fd.append('__RequestVerificationToken', token);

      const xhr = new XMLHttpRequest();
      xhr.open('POST', url, true);
      xhr.withCredentials = true;
      xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      xhr.setRequestHeader('Accept', 'application/json');
      if (token) xhr.setRequestHeader('RequestVerificationToken', token);
      xhr.upload.onprogress = (e) => {
        if (e.lengthComputable && typeof opts.onProgress === 'function') {
          opts.onProgress(Math.round((e.loaded / e.total) * 100));
        }
      };
      xhr.onload = () => {
        let body = null;
        try {
          body = xhr.responseText ? JSON.parse(xhr.responseText) : null;
        } catch (e) {
          body = null;
        }
        if (xhr.status >= 200 && xhr.status < 300) {
          resolve(body);
        } else {
          try {
            reject(handleFailure(xhr.status, body, !!opts.silent));
          } catch (e) {
            reject(e);
          }
        }
      };
      xhr.onerror = () => reject(makeError('網絡異常', 0, null));
      xhr.send(fd);
    });

  async function getText(url, opts = {}) {
    const headers = { 'X-Requested-With': 'XMLHttpRequest' };
    const response = await fetch(url, { headers, credentials: 'same-origin' });
    if (!response.ok) {
      throw handleFailure(response.status, null, !!opts.silent);
    }
    return response.text();
  }

  Ta.http = { get, postForm, post: postForm, upload, getText, request, buildQuery, toFormData };
}
