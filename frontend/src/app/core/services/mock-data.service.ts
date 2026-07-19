import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MockDataService {
  private http = inject(HttpClient);

  getBillingData(): Observable<any> {
    return this.http.get('/data/billing.json');
  }

  getPatientDashboardData(): Observable<any> {
    return this.http.get('/data/patient-dashboard.json');
  }

  getAppointmentsData(): Observable<any> {
    return this.http.get('/data/appointments.json');
  }

  getRecordsData(): Observable<any> {
    return this.http.get('/data/records.json');
  }

  getMessagesData(): Observable<any> {
    return this.http.get('/data/messages.json');
  }

  getAnalyticsData(): Observable<any> {
    return this.http.get('/data/analytics.json');
  }

  getNewConsultationData(): Observable<any> {
    return this.http.get('/data/new-consultation.json');
  }
}
