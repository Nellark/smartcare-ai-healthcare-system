import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Appointment, UpsertAppointmentRequest } from '../models/appointment.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private readonly baseUrl = `${environment.apiBaseUrl}/appointments`;

  constructor(private http: HttpClient) {}

  getAll(patientId?: string | null, doctorId?: string | null): Observable<ApiResponse<Appointment[]>> {
    let params = new HttpParams();

    if (patientId) {
      params = params.set('patientId', patientId);
    }

    if (doctorId) {
      params = params.set('doctorId', doctorId);
    }

    return this.http.get<ApiResponse<Appointment[]>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<ApiResponse<Appointment>> {
    return this.http.get<ApiResponse<Appointment>>(`${this.baseUrl}/${id}`);
  }

  create(appointment: UpsertAppointmentRequest): Observable<ApiResponse<Appointment>> {
    return this.http.post<ApiResponse<Appointment>>(this.baseUrl, appointment);
  }

  update(id: string, appointment: UpsertAppointmentRequest): Observable<ApiResponse<Appointment>> {
    return this.http.put<ApiResponse<Appointment>>(`${this.baseUrl}/${id}`, appointment);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
