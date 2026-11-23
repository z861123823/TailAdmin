/**
 * taMultiSelect：远程搜索 + 多选 + 已选标签，提交为同名多值（FormData 依次 append）。
 */
export function installMultiSelect(Ta) {
  return function taMultiSelect(config) {
    const cfg = config || {};
    return {
      url: cfg.url || '',
      name: cfg.name || 'ids',
      value: cfg.value || [],
      max: cfg.max || 0,
      minChars: cfg.minChars === undefined ? 1 : cfg.minChars,
      placeholder: cfg.placeholder || '請選擇',
      allowClear: cfg.allowClear !== false,
      keyword: '',
      open: false,
      loading: false,
      options: [],
      selected: [],

      async init() {
        if (this.value && this.value.length) {
          try {
            const res = await Ta.http.get(this.url, { ids: this.value.join(',') }, { silent: true });
            const list = (res && res.Data) || [];
            this.selected = this.value.map((val) => {
              const hit = list.find((opt) => String(opt.Value) === String(val));
              return { value: val, label: hit ? hit.Label : String(val) };
            });
          } catch (e) {
            this.selected = this.value.map((val) => ({ value: val, label: String(val) }));
          }
        }
      },

      async search() {
        const keyword = (this.keyword || '').trim();
        if (keyword.length < this.minChars && this.minChars > 0) {
          this.options = [];
          return;
        }
        this.loading = true;
        try {
          const res = await Ta.http.get(this.url, { keyword, page: 1, limit: 20 }, { silent: true });
          this.options = (res && res.Data) || [];
        } catch (e) {
          this.options = [];
        } finally {
          this.loading = false;
        }
      },

      isSelected(value) {
        return this.selected.some((item) => String(item.value) === String(value));
      },

      select(option) {
        if (!option) return;
        if (option.Disabled) return;
        if (this.isSelected(option.Value)) {
          this.remove(option.Value);
          return;
        }
        if (this.max > 0 && this.selected.length >= this.max) {
          Ta.toast.warning(`最多選擇 ${this.max} 項`);
          return;
        }
        this.selected = this.selected.concat([{ value: option.Value, label: option.Label }]);
        this.keyword = '';
        this.options = [];
        this.open = false;
      },

      remove(value) {
        this.selected = this.selected.filter((item) => String(item.value) !== String(value));
      },

      clear() {
        if (!this.allowClear) return;
        this.selected = [];
      },

      onFocus() {
        this.open = true;
        if (!this.options.length) this.search();
      }
    };
  };
}
