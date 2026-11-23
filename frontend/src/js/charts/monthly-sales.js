import ApexCharts from 'apexcharts';
import { brandColor } from './brand-color';

// ===== Monthly Sales Chart（TailAdmin 官方 eCommerce 仪表盘）
const initMonthlySalesChart = () => {
  const el = document.querySelector('#monthlySalesChart');
  if (!el) return;

  const chart = new ApexCharts(el, {
    colors: [brandColor('500')],
    chart: {
      fontFamily: 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif',
      type: 'bar',
      height: 200,
      toolbar: { show: false }
    },
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: '38%',
        borderRadius: 8,
        borderRadiusApplication: 'around',
        dataLabels: { position: 'top' }
      }
    },
    dataLabels: {
      enabled: true,
      formatter: (val) => `${(val / 100).toFixed(1)}%`,
      offsetY: -20,
      style: {
        fontSize: '12px',
        fontFamily: 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif',
        fontWeight: 400,
        colors: ['#364153']
      },
      dropShadow: { enabled: false }
    },
    stroke: { show: false },
    series: [
      {
        name: 'Sales',
        data: [230, 310, 270, 390, 210, 170, 260, 270, 150, 80, 180, 120]
      }
    ],
    xaxis: {
      categories: [
        '1月', '2月', '3月', '4月', '5月', '6月',
        '7月', '8月', '9月', '10月', '11月', '12月'
      ],
      axisBorder: { show: false },
      axisTicks: { show: false },
      labels: {
        style: {
          colors: '#364153',
          fontFamily: 'Outfit, "PingFang SC", "Microsoft YaHei", sans-serif'
        }
      }
    },
    legend: { show: false },
    yaxis: {
      max: 420,
      min: 0,
      tickAmount: 4,
      labels: { show: false }
    },
    grid: {
      borderColor: '#F3F4F6',
      strokeDashArray: 0,
      padding: { left: -10, right: 0 },
      yaxis: { lines: { show: true } },
      xaxis: { lines: { show: false } }
    },
    fill: {
      type: 'gradient',
      gradient: {
        type: 'vertical',
        shadeIntensity: 1,
        opacityFrom: 1,
        opacityTo: 1,
        stops: [0, 100],
        colorStops: [
          { offset: 0, color: brandColor('400', '#7592ff'), opacity: 1 },
          { offset: 100, color: brandColor('500'), opacity: 1 }
        ]
      }
    },
    tooltip: {
      x: { show: false },
      y: { formatter: (val) => `${(val / 100).toFixed(1)}%` }
    }
  });

  chart.render();
};

export default initMonthlySalesChart;
