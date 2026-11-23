import ApexCharts from 'apexcharts';
import { brandColor } from './brand-color';

// ===== Monthly Target Chart（TailAdmin 官方 eCommerce 仪表盘）
const initMonthlyTargetChart = () => {
  const el = document.querySelector('#monthlyTargetChart');
  if (!el) return;

  const isDarkMode = () =>
    document.documentElement.classList.contains('dark') ||
    document.body.classList.contains('dark');

  const getOptions = (dark) => ({
    series: [75.55],
    colors: [brandColor('300', '#9cb9ff')],
    chart: {
      fontFamily: 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif',
      type: 'radialBar',
      height: 330,
      sparkline: { enabled: true }
    },
    plotOptions: {
      radialBar: {
        startAngle: -90,
        endAngle: 90,
        hollow: { size: '80%' },
        track: {
          background: dark ? '#1C243D' : '#E8EAEE',
          strokeWidth: '100%',
          margin: 5
        },
        dataLabels: {
          name: { show: false },
          value: {
            fontSize: '36px',
            fontWeight: '600',
            offsetY: -40,
            color: dark ? '#FFFFFF' : '#1D2939',
            formatter: (val) => `${val}%`
          }
        }
      }
    },
    fill: {
      type: 'gradient',
      gradient: {
        shade: 'light',
        type: 'horizontal',
        shadeIntensity: 0.5,
        gradientToColors: [brandColor('500')],
        inverseColors: false,
        opacityFrom: 1,
        opacityTo: 1,
        stops: [0, 100],
        colorStops: [
          { offset: 0, color: brandColor('300', '#9cb9ff'), opacity: 1 },
          { offset: 100, color: brandColor('500'), opacity: 1 }
        ]
      }
    },
    stroke: { lineCap: 'round' },
    labels: ['Progress']
  });

  const chart = new ApexCharts(el, getOptions(isDarkMode()));
  chart.render();

  const observer = new MutationObserver(() => {
    chart.updateOptions(getOptions(isDarkMode()));
  });
  observer.observe(document.body, { attributes: true, attributeFilter: ['class'] });
  observer.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
};

export default initMonthlyTargetChart;
