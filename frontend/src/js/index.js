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

Ta.version = '1.1.0';
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

document.addEventListener('DOMContentLoaded', () => {
  initMonthlySalesChart();
  initMonthlyTargetChart();
  initStatisticsChart();
});
