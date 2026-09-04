import { Component, OnInit, AfterViewInit, OnDestroy, ElementRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as echarts from 'echarts';
import { MockDataService } from '../../core/services/mock-data.service';

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
export class AnalyticsComponent implements OnInit, AfterViewInit, OnDestroy {
  private mockDataService = inject(MockDataService);

  @ViewChild('trendsChart') trendsChartRef!: ElementRef;
  @ViewChild('donutChart') donutChartRef!: ElementRef;

  private trendsChart?: echarts.ECharts;
  private donutChart?: echarts.ECharts;

  exportLogs: ExportLog[] = [];
  chartData: any = null;
  private isViewInit = false;

  ngOnInit(): void {
    this.mockDataService.getAnalyticsData().subscribe({
      next: (data) => {
        this.exportLogs = data.exportLogs;
        this.chartData = data;
        this.tryInitCharts();
      }
    });
  }

  ngAfterViewInit(): void {
    this.isViewInit = true;
    this.tryInitCharts();
  }

  private tryInitCharts(): void {
    if (this.isViewInit && this.chartData) {
      setTimeout(() => {
        this.initTrendsChart(this.chartData.trendsChart);
        this.initDonutChart(this.chartData.donutChart);
      }, 0);
    }
  }

  private initTrendsChart(data: any): void {
    if (!data) return;
    this.trendsChart = echarts.init(this.trendsChartRef.nativeElement);
    this.trendsChart.setOption({
      tooltip: { trigger: 'axis' },
      grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        data: data.xAxisData,
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
          data: data.neutralSeries,
          lineStyle: { width: 2, type: 'dashed', color: '#52A2FF' },
          symbol: 'circle',
          symbolSize: 8,
          itemStyle: { color: '#fff', borderColor: '#52A2FF', borderWidth: 2 }
        },
        {
          name: 'Positive',
          type: 'line',
          data: data.positiveSeries,
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

  private initDonutChart(data: any[]): void {
    if (!data) return;
    this.donutChart = echarts.init(this.donutChartRef.nativeElement);
    const formattedData = data.map((d: any) => ({
      value: d.value,
      name: d.name,
      itemStyle: { color: d.color }
    }));
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
          data: formattedData
        }
      ]
    });
  }

  ngOnDestroy(): void {
    this.trendsChart?.dispose();
    this.donutChart?.dispose();
  }
}
