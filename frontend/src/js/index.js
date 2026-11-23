import Alpine from 'alpinejs';
import { installHttp } from './core/http';
import { installFormat } from './core/format';
import { installLegacy } from './core/legacy';
import { installTheme } from './core/theme';
import { installToast } from './components/toast';
import { installConfirm } from './components/confirm';
import { installModal } from './components/modal';
import { installForm } from './components/form';
import { installSearch } from './components/search';
import { installUploader } from './components/uploader';
import { installMultiSelect } from './components/multiselect';
import { installDatepicker } from './components/datepicker';
import { createTaTable } from './components/table';

const Ta = {
  config: window.TaConfig || {},
  http: {},
  format: {},
  theme: {},
  toast: {},
  modal: {},
  form: {}
};

installHttp(Ta);
installFormat(Ta);
installLegacy(Ta);
installToast(Ta);
installConfirm(Ta);
const createModal = installModal(Ta);
const createForm = installForm(Ta);
installTheme(Ta);

Ta.version = '1.0.0';
window.Ta = Ta;

const createSearch = installSearch(Ta);
const createUploader = installUploader(Ta);
const createMultiSelect = installMultiSelect(Ta);
const createTable = createTaTable(Ta);

// 侧边栏折叠状态在首屏恢复，避免刷新后跳变
try {
  const savedCollapsed = localStorage.getItem('ta-sidebar-collapsed');
  if (savedCollapsed === '1') Ta.config.sidebarCollapsed = true;
  else if (savedCollapsed === '0') Ta.config.sidebarCollapsed = false;
} catch (e) {
  /* ignore */
}

document.addEventListener('alpine:init', () => {
  Alpine.store('taUi', {
    sidebarOpen: false,
    collapsed: Ta.config.sidebarCollapsed === true,
    toggleSidebar() {
      if (window.matchMedia('(min-width: 1024px)').matches) {
        this.collapsed = !this.collapsed;
        try {
          localStorage.setItem('ta-sidebar-collapsed', this.collapsed ? '1' : '0');
        } catch (e) {
          /* ignore */
        }
      } else {
        this.sidebarOpen = !this.sidebarOpen;
      }
    },
    closeSidebar() {
      this.sidebarOpen = false;
    }
  });

  Alpine.data('taTable', (config) => createTable(config));
  Alpine.data('taModal', (config) => createModal(config));
  Alpine.data('taSearch', (config) => createSearch(config));
  Alpine.data('taUploader', (config) => createUploader(config));
  Alpine.data('taMultiSelect', (config) => createMultiSelect(config));
  Alpine.data('taForm', () => createForm());

  installDatepicker(Alpine);
});

Ta.theme.init();
Alpine.start();
