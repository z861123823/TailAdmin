/**
 * taSearch：列表页顶部「查询」按钮 + 弹窗输入框（回车确认，留空查询全部）。
 */
export function installSearch(Ta) {
  return function taSearch(config) {
    const cfg = config || {};
    return {
      tableId: cfg.tableId,
      placeholder: cfg.placeholder || '輸入關鍵字，留空查詢全部',
      keyword: '',
      lastKeyword: '',

      open() {
        this.keyword = this.lastKeyword;
        if (this.$refs.dlg) this.$refs.dlg.showModal();
        this.$nextTick(() => {
          if (this.$refs.kw) this.$refs.kw.focus();
        });
      },

      close() {
        if (this.$refs.dlg) this.$refs.dlg.close();
      },

      reset() {
        this.keyword = '';
        if (this.$refs.kw) this.$refs.kw.focus();
      },

      confirm() {
        this.lastKeyword = this.keyword;
        document.dispatchEvent(
          new CustomEvent('ta:table-filter', {
            detail: { tableId: this.tableId, keyword: this.lastKeyword }
          })
        );
        this.close();
      }
    };
  };
}
