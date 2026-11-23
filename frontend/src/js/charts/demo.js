import ApexCharts from 'apexcharts';
import { brandColor } from './brand-color';

const fontFamily = 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif';
const months = ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'];

const baseGrid = { borderColor: '#e5e7eb', strokeDashArray: 4 };

function initLineChart() {
  const el = document.querySelector('#demoLineChart');
  if (!el) return;

  new ApexCharts(el, {
    chart: { type: 'area', height: 320, fontFamily, toolbar: { show: false } },
    colors: [brandColor('500'), '#12b76a'],
    series: [
      { name: '本期', data: [31, 40, 28, 51, 42, 72, 65, 88, 74, 96, 84, 112] },
      { name: '上期', data: [22, 32, 21, 38, 30, 55, 48, 66, 58, 71, 63, 86] }
    ],
    stroke: { curve: 'smooth', width: 2 },
    fill: { type: 'gradient', gradient: { opacityFrom: 0.35, opacityTo: 0.03 } },
    dataLabels: { enabled: false },
    xaxis: { categories: months, axisBorder: { show: false }, axisTicks: { show: false } },
    yaxis: { labels: { formatter: (value) => `${value}k` } },
    grid: baseGrid,
    legend: { position: 'top', horizontalAlign: 'right' }
  }).render();
}

function initBarChart() {
  const el = document.querySelector('#demoBarChart');
  if (!el) return;

  new ApexCharts(el, {
    chart: { type: 'bar', height: 320, fontFamily, toolbar: { show: false } },
    colors: [brandColor('500'), '#f79009'],
    series: [
      { name: '線上', data: [44, 55, 57, 56, 61, 58, 63, 60, 66, 72, 68, 75] },
      { name: '門店', data: [35, 41, 36, 26, 45, 48, 52, 53, 41, 55, 49, 58] }
    ],
    plotOptions: {
      bar: { horizontal: false, columnWidth: '45%', borderRadius: 6, borderRadiusApplication: 'around' }
    },
    dataLabels: { enabled: false },
    xaxis: { categories: months, axisBorder: { show: false }, axisTicks: { show: false } },
    grid: baseGrid,
    legend: { position: 'top', horizontalAlign: 'right' }
  }).render();
}

function initPieChart() {
  const el = document.querySelector('#demoPieChart');
  if (!el) return;

  new ApexCharts(el, {
    chart: { type: 'donut', height: 320, fontFamily },
    colors: [brandColor('500'), '#12b76a', '#f79009', '#f04438'],
    series: [42, 28, 18, 12],
    labels: ['服飾', '數碼', '家居', '其他'],
    stroke: { width: 2, colors: ['#ffffff'] },
    dataLabels: { enabled: false },
    legend: { position: 'bottom', horizontalAlign: 'center' },
    plotOptions: {
      pie: { donut: { size: '68%', labels: { show: true, total: { show: true, label: '合計', formatter: () => '100%' } } } }
    }
  }).render();
}

function initTrafficDonut() {
  const el = document.querySelector('#demoTrafficChart');
  if (!el) return;

  new ApexCharts(el, {
    chart: { type: 'donut', height: 280, fontFamily },
    colors: [brandColor('500'), '#12b76a', '#f79009', '#98a2b3'],
    series: [58, 32, 7, 3],
    labels: ['桌面', '移動', '平板', '其他'],
    stroke: { width: 2, colors: ['#ffffff'] },
    dataLabels: { enabled: false },
    legend: { position: 'bottom', horizontalAlign: 'center' },
    plotOptions: {
      pie: { donut: { size: '70%', labels: { show: true, total: { show: true, label: '合計', formatter: () => '100%' } } } }
    }
  }).render();
}

function initPlanDonut() {
  const el = document.querySelector('#demoPlanChart');
  if (!el) return;

  new ApexCharts(el, {
    chart: { type: 'donut', height: 280, fontFamily },
    colors: [brandColor('500'), '#12b76a', '#f79009'],
    series: [62, 28, 10],
    labels: ['企業版', '專業版', '基礎版'],
    stroke: { width: 2, colors: ['#ffffff'] },
    dataLabels: { enabled: false },
    legend: { position: 'bottom', horizontalAlign: 'center' },
    plotOptions: {
      pie: { donut: { size: '70%', labels: { show: true, total: { show: true, label: 'MRR', formatter: () => '$48.2k' } } } }
    }
  }).render();
}

export default function initDemoCharts() {
  initLineChart();
  initBarChart();
  initPieChart();
  initTrafficDonut();
  initPlanDonut();
}
