import collapse from '@alpinejs/collapse';
import persist from '@alpinejs/persist';
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
import initMonthlySalesChart from './charts/monthly-sales';
import initMonthlyTargetChart from './charts/monthly-target';
import initStatisticsChart from './charts/statistics';

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

Ta.version = '1.5.0';
Ta.alpine = Alpine;
window.Ta = Ta;

const createSearch = installSearch(Ta);
const createUploader = installUploader(Ta);
const createMultiSelect = installMultiSelect(Ta);
const createTable = createTaTable(Ta);

Alpine.plugin(collapse);
Alpine.plugin(persist);

document.addEventListener('alpine:init', () => {
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

// 原生 <dialog> 通用关闭行为：
// 1) 点击遮罩（事件目标是 dialog 本身）关闭；
// 2) 任意带 data-dialog-close 的按钮关闭最近的 dialog。
document.addEventListener('click', (event) => {
  const target = event.target;
  if (target instanceof HTMLDialogElement && target.classList.contains('ta-dialog') && target.open) {
    target.close();
    return;
  }

  const trigger = target instanceof Element ? target.closest('[data-dialog-close]') : null;
  if (trigger) {
    const dialog = trigger.closest('dialog');
    if (dialog && dialog.open) dialog.close();
  }
});

document.addEventListener('DOMContentLoaded', () => {
  initMonthlySalesChart();
  initMonthlyTargetChart();
  initStatisticsChart();
});
