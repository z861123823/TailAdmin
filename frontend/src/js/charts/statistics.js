import ApexCharts from 'apexcharts';
import { brandColor } from './brand-color';

// ===== Statistics Chart（TailAdmin 官方 eCommerce 仪表盘）
const initStatisticsChart = () => {
  const el = document.querySelector('#statisticsChart');
  if (!el) return;

  const categories = [
    '1月', '2月', '3月', '4月', '5月', '6月',
    '7月', '8月', '9月', '10月', '11月', '12月'
  ];
  const salesData = [180, 190, 170, 160, 175, 165, 170, 205, 230, 210, 240, 235];
  const revenueData = [40, 30, 50, 40, 55, 40, 70, 100, 110, 120, 150, 140];

  let currentTab = 'overview';
  const isDarkMode = () => document.documentElement.classList.contains('dark');

  const getSeriesAndColors = (tab) => {
    if (tab === 'sales') {
      return {
        series: [{ name: 'Sales', data: salesData }],
        colors: [brandColor('500')]
      };
    }
    if (tab === 'revenue') {
      return {
        series: [{ name: 'Revenue', data: revenueData }],
        colors: [brandColor('300', '#9cb9ff')]
      };
    }
    return {
      series: [
        { name: 'Sales', data: salesData },
        { name: 'Revenue', data: revenueData }
      ],
      colors: [brandColor('500'), brandColor('300', '#9cb9ff')]
    };
  };

  const initial = getSeriesAndColors(currentTab);

  const options = {
    series: initial.series,
    colors: initial.colors,
    legend: { show: false },
    chart: {
      fontFamily: 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif',
      height: 310,
      type: 'area',
      toolbar: { show: false }
    },
    fill: {
      type: 'gradient',
      gradient: { opacityFrom: 0.55, opacityTo: 0 }
    },
    stroke: { curve: 'straight', width: [2, 2] },
    markers: {
      size: 0,
      strokeColors: '#fff',
      strokeWidth: 2,
      hover: { size: 6 }
    },
    grid: {
      borderColor: isDarkMode() ? '#1E2640' : '#F3F4F6',
      strokeDashArray: 0,
      xaxis: { lines: { show: false } },
      yaxis: { lines: { show: true } }
    },
    dataLabels: { enabled: false },
    tooltip: {
      enabled: true,
      shared: true,
      intersect: false,
      custom: ({ series, dataPointIndex, w }) => {
        const category =
          (w.globals && w.globals.categoryLabels && w.globals.categoryLabels[dataPointIndex]) ||
          (w.config.xaxis && w.config.xaxis.categories && w.config.xaxis.categories[dataPointIndex]) ||
          '';
        const list = w.config.series || [];
        const colors = w.config.colors || [];
        const items = list
          .map((item, idx) => {
            const val = series[idx] ? series[idx][dataPointIndex] : undefined;
            if (val === undefined || val === null) return '';
            const formatted = typeof val === 'number' ? val.toLocaleString() : String(val);
            return `
              <div class="flex items-center justify-between gap-3">
                <div class="flex items-center gap-1.5">
                  <span class="size-1.5 shrink-0 rounded-full" style="background-color:${colors[idx] || brandColor('500')}"></span>
                  <span class="text-xs text-gray-800 dark:text-white/90">${item.name}</span>
                  <span class="text-xs font-medium text-gray-400 dark:text-gray-500">:</span>
                </div>
                <span class="text-xs font-medium text-gray-800 dark:text-white/90">${formatted}</span>
              </div>`;
          })
          .join('');
        return `
          <div class="min-w-32.5 space-y-1 rounded-lg border border-gray-200 bg-white p-2 shadow-theme-md dark:border-gray-800 dark:bg-gray-900">
            <p class="mb-1 text-[10px] leading-4 text-gray-500 dark:text-gray-400">${category}</p>
            ${items}
          </div>`;
      }
    },
    xaxis: {
      type: 'category',
      categories,
      axisBorder: { show: false },
      axisTicks: { show: false },
      crosshairs: {
        show: true,
        width: 2,
        position: 'back',
        opacity: 1,
        stroke: { width: 0, color: 'none' },
        fill: {
          type: 'gradient',
          gradient: {
            colorFrom: isDarkMode() ? 'rgba(255, 255, 255, 0.1)' : '#ffffff',
            colorTo: brandColor('300', '#9cb9ff'),
            stops: [0, 100],
            opacityFrom: 1,
            opacityTo: 1
          }
        }
      },
      tooltip: { enabled: false },
      labels: {
        style: {
          fontSize: '12px',
          fontFamily: 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif',
          colors: isDarkMode() ? '#94A3B8' : '#6B7280'
        }
      }
    },
    yaxis: {
      labels: {
        style: {
          fontSize: '12px',
          fontFamily: 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif',
          colors: isDarkMode() ? '#94A3B8' : '#6B7280'
        }
      },
      title: { text: '', style: { fontSize: '0px' } }
    }
  };

  const chart = new ApexCharts(el, options);
  chart.render();

  new MutationObserver(() => {
    const dark = isDarkMode();
    chart.updateOptions({
      grid: { borderColor: dark ? '#1E2640' : '#F3F4F6' },
      xaxis: {
        labels: { style: { colors: dark ? '#94A3B8' : '#6B7280' } },
        crosshairs: {
          fill: {
            gradient: { colorFrom: dark ? 'rgba(255, 255, 255, 0.1)' : '#ffffff' }
          }
        }
      },
      yaxis: {
        labels: { style: { colors: dark ? '#94A3B8' : '#6B7280' } }
      }
    });
  }).observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });

  window.updateStatisticsChart = ({ tab }) => {
    if (tab) currentTab = tab;
    const next = getSeriesAndColors(currentTab);
    chart.updateOptions({
      xaxis: { categories },
      colors: next.colors
    });
    chart.updateSeries(next.series);
  };
};

export default initStatisticsChart;
