import { createTable, getCoreRowModel } from '@tanstack/table-core';

/**
 * taTable：服务端分页 / 排序 / 筛选的表格组件。
 * TanStack Table Core 只负责列模型与状态骨架（manual 模式），渲染完全交给 Alpine + Razor。
 */
export function createTaTable(Ta) {
  return function taTable(config) {
    const cfg = config || {};
    return {
      id: cfg.id || 'taTable',
      url: cfg.url || '',
      columns: cfg.columns || [],
      actions: cfg.actions || [],
      emptyText: cfg.emptyText || '暫無數據',
      rows: [],
      total: 0,
      page: 1,
      limit: cfg.pageSize || 20,
      pageSizes: cfg.pageSizes || [10, 20, 50, 100],
      loading: false,
      error: '',
      keyword: '',
      sortField: cfg.sortField || '',
      sortOrder: cfg.sortOrder || 'desc',
      params: cfg.params || {},
      selected: [],
      table: null,

      init() {
        const columns = this.columns.map((col) => ({
          id: col.key,
          accessorKey: col.key,
          header: col.label || col.key,
          enableSorting: col.sortable !== false,
          meta: col
        }));
        this.table = createTable({
          data: [],
          columns,
          state: { sorting: [], pagination: { pageIndex: 0, pageSize: this.limit } },
          manualPagination: true,
          manualSorting: true,
          manualFiltering: true,
          pageCount: -1,
          getCoreRowModel: getCoreRowModel(),
          onStateChange: () => {}
        });

        this._onFilter = (event) => {
          const detail = event.detail || {};
          if (detail.tableId !== this.id) return;
          this.keyword = detail.keyword || '';
          this.page = 1;
          this.load();
        };
        document.addEventListener('ta:table-filter', this._onFilter);

        this._onModal = (event) => {
          const modalId = event.detail && event.detail.modalId;
          if (modalId && (cfg.reloadOn || []).indexOf(modalId) >= 0) this.refresh();
        };
        document.addEventListener('ta:modal-submitted', this._onModal);

        Ta.tables = Ta.tables || {};
        Ta.tables[this.id] = this;
      },

      destroy() {
        if (this._onFilter) document.removeEventListener('ta:table-filter', this._onFilter);
        if (this._onModal) document.removeEventListener('ta:modal-submitted', this._onModal);
        if (Ta.tables) delete Ta.tables[this.id];
      },

      async load() {
        this.loading = true;
        this.error = '';
        try {
          const query = Object.assign({}, this.params, {
            page: this.page,
            limit: this.limit,
            sortField: this.sortField,
            sortOrder: this.sortOrder,
            keyword: this.keyword
          });
          const method = (cfg.method || 'GET').toUpperCase();
          const res =
            method === 'POST'
              ? await Ta.http.postForm(this.url, query, { silent: true })
              : await Ta.http.get(this.url, query, { silent: true });

          this.rows = res && Array.isArray(res.data) ? res.data : [];
          this.total = res && typeof res.count === 'number' ? res.count : 0;
          if (this.table) this.table.setOptions({ data: this.rows, pageCount: this.pageCount() });
          if (this.rows.length === 0 && this.page > 1) {
            this.page = Math.max(1, this.pageCount());
            await this.load();
          }
        } catch (e) {
          this.error = e && e.message ? e.message : '加載失敗';
          this.rows = [];
          this.total = 0;
        } finally {
          this.loading = false;
        }
      },

      reload() {
        this.page = 1;
        return this.load();
      },

      refresh() {
        return this.load();
      },

      setPage(page) {
        const next = Math.min(Math.max(1, page), this.pageCount());
        if (next === this.page) return;
        this.page = next;
        this.load();
      },

      setLimit(limit) {
        this.limit = Number(limit) || 20;
        this.page = 1;
        this.load();
      },

      applyFilters(filters) {
        this.params = Object.assign({}, this.params, filters || {});
        this.page = 1;
        this.load();
      },

      pageCount() {
        return Math.max(1, Math.ceil(this.total / this.limit));
      },

      pageList() {
        const count = this.pageCount();
        if (count <= 7) return Array.from({ length: count }, (_, i) => i + 1);
        const current = this.page;
        const pages = new Set([1, count, current, current - 1, current + 1]);
        if (current <= 3) [2, 3, 4].forEach((n) => pages.add(n));
        if (current >= count - 2) [count - 1, count - 2, count - 3].forEach((n) => pages.add(n));
        const sorted = Array.from(pages)
          .filter((n) => n >= 1 && n <= count)
          .sort((a, b) => a - b);
        const result = [];
        sorted.forEach((n, index) => {
          if (index > 0 && n - sorted[index - 1] > 1) result.push('...');
          result.push(n);
        });
        return result;
      },

      toggleSort(column) {
        if (!column || column.sortable === false) return;
        if (this.sortField === column.key) {
          this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
        } else {
          this.sortField = column.key;
          this.sortOrder = 'asc';
        }
        this.page = 1;
        this.load();
      },

      sortIcon(key) {
        if (this.sortField !== key) return '↕';
        return this.sortOrder === 'asc' ? '↑' : '↓';
      },

      cellValue(row, column) {
        return row ? row[column.key] : null;
      },

      cellText(row, column) {
        const value = this.cellValue(row, column);
        if (column.formatter && typeof Ta.format[column.formatter] === 'function') {
          return Ta.format[column.formatter](value, row, column);
        }
        switch (column.type) {
          case 'money':
            return Ta.format.money(value);
          case 'number':
            return Ta.format.number(value);
          case 'date':
            return Ta.format.date(value);
          case 'bool':
            return Ta.format.bool(value);
          default:
            return value === null || value === undefined ? '' : String(value);
        }
      },

      alignClass(column) {
        if (column.align === 'right') return 'text-right';
        if (column.align === 'center') return 'text-center';
        return '';
      },

      cellClass(column) {
        return [this.alignClass(column), column.cellClass || ''].filter(Boolean).join(' ');
      },

      badgeClass(row, column) {
        const value = this.cellValue(row, column);
        const truthy = value === true || value === 'true' || value === 1 || value === '1';
        if (column.type === 'bool') {
          return truthy
            ? 'bg-green-50 text-green-600 dark:bg-green-500/15 dark:text-green-500'
            : 'bg-gray-100 text-gray-700 dark:bg-white/5 dark:text-white/80';
        }
        return 'bg-brand-50 text-brand-600 dark:bg-brand-500/15 dark:text-brand-400';
      },

      imageUrl(row, column) {
        const value = this.cellValue(row, column);
        if (!value) return '';
        return String(value).split(',')[0].trim();
      },

      linkHref(row, column) {
        const value = this.cellValue(row, column);
        return value ? String(value) : '';
      },

      preview(url) {
        Ta.previewImage(url);
      },

      rowId(row) {
        return row ? row[cfg.rowKey || 'Id'] : null;
      },

      isSelected(row) {
        const id = this.rowId(row);
        return this.selected.some((item) => String(item) === String(id));
      },

      toggleRow(row) {
        const id = this.rowId(row);
        if (this.isSelected(row)) {
          this.selected = this.selected.filter((item) => String(item) !== String(id));
        } else {
          this.selected = this.selected.concat([id]);
        }
      },

      toggleAll(event) {
        if (event && event.target && event.target.checked) {
          this.selected = this.rows.map((row) => this.rowId(row));
        } else {
          this.selected = [];
        }
      },

      doAction(name, row) {
        const handlers = cfg.onAction || {};
        const handler = handlers[name];
        if (typeof handler === 'function') {
          handler(row, this);
        } else {
          Ta.toast.warning(`未實現的操作：${name}`);
        }
      }
    };
  };
}
