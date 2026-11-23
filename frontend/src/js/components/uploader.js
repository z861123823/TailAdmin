/**
 * taUploader：图片/附件上传，支持预览、删除、进度、体积与类型校验。
 * 上传接口返回：{ Success: true, Message, Data: { PicId, PicName, PicUrl } }
 */
export function installUploader(Ta) {
  return function taUploader(config) {
    const cfg = config || {};
    return {
      url: cfg.url || '',
      fieldName: cfg.name || 'file',
      hiddenName: cfg.hiddenName || 'PicUrl',
      accept: cfg.accept || 'image/*',
      maxSize: cfg.maxSize || 2 * 1024 * 1024,
      max: cfg.max || 1,
      multiple: !!cfg.multiple,
      preview: cfg.preview !== false,
      extraParams: cfg.extraParams || {},
      base64Fallback: !!cfg.base64Fallback,
      items: [],
      uploading: false,
      error: '',

      init(existing) {
        const list = Array.isArray(existing) ? existing : [];
        this.items = list
          .map((item, index) => {
            if (!item) return null;
            const url = typeof item === 'string' ? item : item.PicUrl || item.url;
            if (!url) return null;
            return {
              uid: `init-${index}-${url}`,
              name: (typeof item === 'object' && (item.PicName || item.name)) || url.split('/').pop(),
              url,
              progress: 100,
              status: 'done'
            };
          })
          .filter(Boolean);
      },

      choose() {
        if (this.uploading) return;
        this.$refs.file.click();
      },

      onChange(event) {
        const files = Array.from(event.target.files || []);
        event.target.value = '';
        if (!files.length) return;
        this.error = '';
        const remain = Math.max(0, this.max - this.items.length);
        if (remain <= 0) {
          this.error = `最多上傳 ${this.max} 個文件`;
          return;
        }
        files.slice(0, remain).forEach((file) => {
          const invalid = this.validate(file);
          if (invalid) {
            this.error = invalid;
            return;
          }
          this.upload(file);
        });
      },

      validate(file) {
        if (file.size > this.maxSize) {
          return `文件「${file.name}」超過 ${Math.round(this.maxSize / 1024)}KB 限制`;
        }
        if (this.accept && this.accept !== '*') {
          const ok = this.accept
            .split(',')
            .map((s) => s.trim())
            .some((rule) => {
              if (rule.endsWith('/*')) return file.type.startsWith(rule.slice(0, -1));
              return file.type === rule || file.name.toLowerCase().endsWith(rule.toLowerCase());
            });
          if (!ok) return `文件「${file.name}」類型不支持`;
        }
        return '';
      },

      async upload(file) {
        const item = {
          uid: `${Date.now()}-${Math.random().toString(16).slice(2)}`,
          name: file.name,
          url: '',
          progress: 0,
          status: 'uploading'
        };
        this.items = this.items.concat([item]);
        this.uploading = true;
        try {
          const res = await this.post(file, item);
          const data = Ta.value(res, 'Data');
          if (Ta.isOk(res) && data) {
            item.url = Ta.value(data, 'PicUrl') || Ta.value(data, 'PicName') || '';
            item.status = 'done';
            item.progress = 100;
            Ta.toast.success(Ta.value(res, 'Message') || '上傳成功');
          } else {
            item.status = 'error';
            this.error = Ta.value(res, 'Message') || '上傳失敗';
            Ta.toast.error(this.error);
          }
        } catch (e) {
          item.status = 'error';
          this.error = e && e.message ? e.message : '上傳失敗';
        } finally {
          this.items = this.items.slice();
          this.uploading = false;
        }
      },

      async post(file, item) {
        if (this.base64Fallback && file.size <= 512 * 1024) {
          const dataUrl = await this.toBase64(file);
          return Ta.http.postForm(this.url, Object.assign({}, this.extraParams, { base64: dataUrl }));
        }
        const fd = new FormData();
        fd.append(this.fieldName, file, file.name);
        Object.keys(this.extraParams).forEach((key) => fd.append(key, this.extraParams[key]));
        return Ta.http.upload(this.url, fd, {
          onProgress: (percent) => {
            item.progress = percent;
            this.items = this.items.slice();
          }
        });
      },

      toBase64(file) {
        return new Promise((resolve, reject) => {
          const reader = new FileReader();
          reader.onload = () => resolve(String(reader.result || ''));
          reader.onerror = () => reject(new Error('讀取文件失敗'));
          reader.readAsDataURL(file);
        });
      },

      remove(uid) {
        this.items = this.items.filter((item) => item.uid !== uid);
      },

      clear() {
        this.items = [];
        this.error = '';
      }
    };
  };
}
