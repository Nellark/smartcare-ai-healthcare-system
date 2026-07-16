import { Component, OnInit, AfterViewInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as echarts from 'echarts';

interface ExportLog {
  name: string;
  date: string;
  status: 'Completed' | 'Expired';
}

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.css']
})
export class AnalyticsComponent implements AfterViewInit, OnDestroy {
  @ViewChild('trendsChart') trendsChartRef!: ElementRef;
  @ViewChild('donutChart') donutChartRef!: ElementRef;

  private trendsChart?: echarts.ECharts;
  private donutChart?: echarts.ECharts;

  exportLogs: ExportLog[] = [
    { name: 'Q3_Patient_Outcome_v2.pdf', date: 'Oct 24, 2023', status: 'Completed' },
    { name: 'Monthly_Clinic_Volume.csv', date: 'Oct 22, 2023', status: 'Completed' },
    { name: 'Demographics_Distribution.pdf', date: 'Oct 20, 2023', status: 'Expired' }
  ];

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.initTrendsChart();
      this.initDonutChart();
    }, 0);
  }

  private initTrendsChart(): void {
    this.trendsChart = echarts.init(this.trendsChartRef.nativeElement);
    this.trendsChart.setOption({
      tooltip: { trigger: 'axis' },
      grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        data: ['Week 1', 'Week 2', 'Week 3', 'Week 4', 'Week 5', 'Week 6'],
        axisLine: { show: false },
        axisTick: { show: false },
        axisLabel: { color: '#64748B', fontSize: 11 }
      },
      yAxis: {
        type: 'value',
        min: 0,
        max: 100,
        splitLine: { lineStyle: { color: '#E2E8F0' } },
        axisLabel: { color: '#64748B', fontSize: 11 }
      },
      series: [
        {
          name: 'Neutral',
          type: 'line',
          data: [40, 45, 52, 48, 50, 58],
          lineStyle: { width: 2, type: 'dashed', color: '#52A2FF' },
          symbol: 'circle',
          symbolSize: 8,
          itemStyle: { color: '#fff', borderColor: '#52A2FF', borderWidth: 2 }
        },
        {
          name: 'Positive',
          type: 'line',
          data: [65, 78, 72, 85, 92, 90],
          lineStyle: { width: 3, color: '#026C7C' },
          areaStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              { offset: 0, color: 'rgba(2, 108, 124, 0.2)' },
              { offset: 1, color: 'rgba(2, 108, 124, 0.0)' }
            ])
          },
          symbol: 'circle',
          symbolSize: 8,
          itemStyle: { color: '#fff', borderColor: '#026C7C', borderWidth: 2 }
        }
      ]
    });
  }

  private initDonutChart(): void {
    this.donutChart = echarts.init(this.donutChartRef.nativeElement);
    this.donutChart.setOption({
      tooltip: { trigger: 'item', formatter: '{b}: {c}%' },
      series: [
        {
          name: 'Demographics',
          type: 'pie',
          radius: ['55%', '78%'],
          avoidLabelOverlap: false,
          label: { show: false },
          labelLine: { show: false },
          data: [
            { value: 24, name: 'Pediatric', itemStyle: { color: '#BBE1FF' } },
            { value: 48, name: 'Adult', itemStyle: { color: '#52A2FF' } },
            { value: 28, name: 'Geriatric', itemStyle: { color: '#026C7C' } }
          ]
        }
      ]
    });
  }

  ngOnDestroy(): void {
    this.trendsChart?.dispose();
    this.donutChart?.dispose();
  }
}
