import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { MedicalRecord, UpsertMedicalRecordRequest } from '../models/medical-record.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MedicalRecordService {
  private readonly baseUrl = `${environment.apiBaseUrl}/medicalrecords`;

  constructor(private http: HttpClient) {}

  getAll(patientId?: string | null): Observable<ApiResponse<MedicalRecord[]>> {
    let params = new HttpParams();

    if (patientId) {
      params = params.set('patientId', patientId);
    }

    return this.http.get<ApiResponse<MedicalRecord[]>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<ApiResponse<MedicalRecord>> {
    return this.http.get<ApiResponse<MedicalRecord>>(`${this.baseUrl}/${id}`);
  }

  create(record: UpsertMedicalRecordRequest): Observable<ApiResponse<MedicalRecord>> {
    return this.http.post<ApiResponse<MedicalRecord>>(this.baseUrl, record);
  }

  update(id: string, record: UpsertMedicalRecordRequest): Observable<ApiResponse<MedicalRecord>> {
    return this.http.put<ApiResponse<MedicalRecord>>(`${this.baseUrl}/${id}`, record);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
