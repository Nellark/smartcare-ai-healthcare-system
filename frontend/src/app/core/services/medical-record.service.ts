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

  upload(record: UpsertMedicalRecordRequest): Observable<ApiResponse<MedicalRecord>> {
    const formData = new FormData();
    formData.append('patientId', record.patientId);
    formData.append('diagnosis', record.diagnosis);
    formData.append('treatment', record.treatment);
    formData.append('notes', record.notes);
    formData.append('recordDate', record.recordDate);
    formData.append('doctorId', record.doctorId);
    
    if (record.recordType) formData.append('recordType', record.recordType);
    if (record.title) formData.append('title', record.title);
    if (record.provider) formData.append('provider', record.provider);
    if (record.status) formData.append('status', record.status);
    if (record.file) formData.append('file', record.file);

    return this.http.post<ApiResponse<MedicalRecord>>(`${this.baseUrl}/upload`, formData);
  }

  update(id: string, record: UpsertMedicalRecordRequest): Observable<ApiResponse<MedicalRecord>> {
    return this.http.put<ApiResponse<MedicalRecord>>(`${this.baseUrl}/${id}`, record);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
