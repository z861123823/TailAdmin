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
      canScrollLeft: false,
      canScrollRight: false,
      offsets: {},
      mode: 'server',
      sourceRows: [],
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
        this.mode = 'server';
        this.loading = true;
        this.error = '';
        this.applyFixedOffsets();
        if (!this.url) {
          this.loading = false;
          return;
        }
        if (document.fonts && document.fonts.ready) {
          document.fonts.ready.then(() => this.applyFixedOffsets());
        }
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
          this.$nextTick(() => {
            this.updateScroll();
            this.applyFixedOffsets();
          });
        }
      },

      reload() {
        this.page = 1;
        if (this.mode === 'local') {
          this.applyLocal();
          return this;
        }
        return this.load();
      },

      refresh() {
        if (this.mode === 'local') {
          this.applyLocal();
          return this;
        }
        return this.load();
      },

      setPage(page) {
        const next = Math.min(Math.max(1, page), this.pageCount());
        if (next === this.page) return;
        this.page = next;
        if (this.mode === 'local') {
          this.applyLocal();
          return;
        }
        this.load();
      },

      setLimit(limit) {
        this.limit = Number(limit) || 20;
        this.page = 1;
        if (this.mode === 'local') {
          this.applyLocal();
          return;
        }
        this.load();
      },

      applyFilters(filters) {
        this.params = Object.assign({}, this.params, filters || {});
        this.page = 1;
        if (this.mode === 'local') {
          this.applyLocal();
          return;
        }
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
        if (this.mode === 'local') {
          this.applyLocal();
          return;
        }
        this.load();
      },

      sortIcon(key) {
        if (this.sortField !== key) return '↕';
        return this.sortOrder === 'asc' ? '↑' : '↓';
      },

      /**
       * 外部喂数据（对齐 layui table.reload(id, { data })）。
       * - 不传 total：本地数据模式，保存全量，前端分页/排序/关键字过滤
       * - 传 total：服务端分页快照，只替换当前页，后续分页/排序仍走 Url
       */
      setData(rows, options) {
        const opts = options || {};
        const list = Array.isArray(rows) ? rows.slice() : [];
        const hasTotal = opts.total !== undefined && opts.total !== null;

        if (opts.limit != null) this.limit = Math.max(1, Number(opts.limit) || this.limit);
        if (opts.page != null) this.page = Math.max(1, Number(opts.page) || 1);
        if (opts.keyword !== undefined) this.keyword = opts.keyword || '';
        if (opts.sortField !== undefined) this.sortField = opts.sortField || '';
        if (opts.sortOrder !== undefined) this.sortOrder = opts.sortOrder || 'desc';
        this.error = '';
        this.loading = false;
        this.selected = opts.keepSelection ? this.selected : [];

        if (hasTotal) {
          this.mode = 'server';
          this.sourceRows = [];
          this.rows = list;
          this.total = Math.max(0, Number(opts.total) || 0);
          const count = this.pageCount();
          if (this.page > count) this.page = count;
          if (opts.keepSelection) this.pruneSelection(this.rows);
          if (this.table) this.table.setOptions({ data: this.rows, pageCount: count });
          this.$nextTick(() => {
            this.updateScroll();
            this.applyFixedOffsets();
          });
          return this;
        }

        this.mode = 'local';
        this.sourceRows = list;
        this.applyLocal();
        return this;
      },

      /** 自定义请求 + setData：返回 count 走服务端快照，否则按本地数据处理。 */
      async loadData(url, params) {
        this.loading = true;
        this.error = '';
        try {
          const res = await Ta.http.get(url, params || {}, { silent: true });
          const list = res && Array.isArray(res.data) ? res.data : [];
          const total = res && typeof res.count === 'number' ? res.count : undefined;
          this.setData(list, {
            total,
            page: params ? params.page : undefined,
            limit: params ? params.limit : undefined,
            keyword: params ? params.keyword : undefined,
            sortField: params ? params.sortField : undefined,
            sortOrder: params ? params.sortOrder : undefined
          });
          return this;
        } catch (e) {
          this.rows = [];
          this.total = 0;
          this.error = (e && e.message) ? e.message : '加載失敗';
          this.loading = false;
          this.$nextTick(() => {
            this.updateScroll();
            this.applyFixedOffsets();
          });
          return this;
        }
      },

      /** 本地数据：前端过滤 + 排序 + 分页，然后重算固定列/滚动状态。 */
      applyLocal() {
        const keyword = (this.keyword || '').trim().toLowerCase();
        let data = Array.isArray(this.sourceRows) ? this.sourceRows.slice() : [];

        if (keyword) {
          data = data.filter((row) =>
            row && Object.keys(row).some((key) => {
              const value = row[key];
              return value !== null && value !== undefined && String(value).toLowerCase().indexOf(keyword) >= 0;
            })
          );
        }

        if (this.sortField) {
          const field = this.sortField;
          const direction = this.sortOrder === 'asc' ? 1 : -1;
          data.sort((a, b) => this.compareValues(a ? a[field] : null, b ? b[field] : null) * direction);
        }

        this.total = data.length;
        const count = this.pageCount();
        if (this.page > count) this.page = count;
        if (this.page < 1) this.page = 1;
        const start = (this.page - 1) * this.limit;
        this.rows = data.slice(start, start + this.limit);

        this.pruneSelection(data);
        if (this.table) this.table.setOptions({ data: this.rows, pageCount: count });

        this.$nextTick(() => {
          this.updateScroll();
          this.applyFixedOffsets();
        });
      },

      /** 本地排序对比：数字按数值、布尔按真假、其余按本地化字符串。 */
      compareValues(a, b) {
        const aEmpty = a === null || a === undefined || a === '';
        const bEmpty = b === null || b === undefined || b === '';
        if (aEmpty && bEmpty) return 0;
        if (aEmpty) return 1;
        if (bEmpty) return -1;
        if (typeof a === 'boolean' || typeof b === 'boolean') {
          return (a ? 1 : 0) - (b ? 1 : 0);
        }
        const na = Number(a);
        const nb = Number(b);
        if (!Number.isNaN(na) && !Number.isNaN(nb)) return na - nb;
        return String(a).localeCompare(String(b), 'zh-Hans-CN', { numeric: true });
      },

      /** 多选残留清理：只保留仍存在于给定数据中的行 id。 */
      pruneSelection(source) {
        const list = Array.isArray(source) ? source : this.rows;
        const ids = new Set(list.map((row) => String(this.rowId(row))));
        this.selected = this.selected.filter((id) => ids.has(String(id)));
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
        const classes = [this.alignClass(column), column.cellClass || ''];
        if (column.fixed === 'left' || column.fixed === 'right') {
          classes.push('sticky z-10 ta-sticky-cell');
        }
        if (column.edgeLeft) classes.push('ta-sticky-edge-left');
        if (column.edgeRight) classes.push('ta-sticky-edge-right');
        return classes.filter(Boolean).join(' ');
      },

      cellStyle(column) {
        const parts = [];
        const offset = this.offsets[column.key] || {};
        const width = offset.width || column.width;
        if (width) parts.push(`width:${width}px`);
        if (column.fixed === 'left') {
          const left = offset.left != null ? offset.left : column.left;
          if (left != null) parts.push(`left:${left}px`);
        }
        if (column.fixed === 'right') {
          const right = offset.right != null ? offset.right : column.right;
          if (right != null) parts.push(`right:${right}px`);
        }
        return parts.join(';');
      },

      /**
       * 按表头实际渲染宽度重算固定列偏移。
       * 静态 Width 会被浏览器自动布局压缩/撑开，必须用真实宽度累加，否则多列固定会出现缝隙或重叠。
       */
      applyFixedOffsets() {
        const scroller = this.$refs.scroller;
        const table = scroller ? scroller.querySelector('table') : null;
        if (!table) return;
        const heads = Array.prototype.slice.call(table.querySelectorAll('thead th'));
        if (!heads.length) return;

        const widths = heads.map((th) => th.getBoundingClientRect().width);
        const offsets = {};

        let left = 0;
        heads.forEach((th, index) => {
          const key = th.dataset.colKey;
          if (!key || th.dataset.fixed !== 'left') return;
          offsets[key] = Object.assign({}, offsets[key], { left, width: widths[index] });
          left += widths[index];
        });

        let right = 0;
        for (let index = heads.length - 1; index >= 0; index -= 1) {
          const th = heads[index];
          const key = th.dataset.colKey;
          if (!key || th.dataset.fixed !== 'right') continue;
          offsets[key] = Object.assign({}, offsets[key], { right, width: widths[index] });
          right += widths[index];
        }

        this.offsets = offsets;

        heads.forEach((th, index) => {
          const key = th.dataset.colKey;
          if (!key) return;
          if (th.dataset.fixed === 'left' && offsets[key]) {
            th.style.left = `${offsets[key].left}px`;
            th.style.width = `${widths[index]}px`;
          } else if (th.dataset.fixed === 'right' && offsets[key]) {
            th.style.right = `${offsets[key].right}px`;
            th.style.width = `${widths[index]}px`;
          }
        });
      },

      updateScroll() {
        const el = this.$refs.scroller;
        if (!el) return;
        this.canScrollLeft = el.scrollLeft > 0;
        this.canScrollRight = el.scrollLeft + el.clientWidth < el.scrollWidth - 1;
        el.classList.toggle('is-stuck', el.scrollTop > 0);
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
