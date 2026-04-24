import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient, CreatePatientRequest, ApiResponse } from '../models/patient.model';

@Injectable({
  providedIn: 'root'
})
export class PatientService {

  private baseUrl = 'http://localhost:5255/api/patients';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Patient[]>> {
    return this.http.get<ApiResponse<Patient[]>>(this.baseUrl);
  }

  getById(id: string): Observable<ApiResponse<Patient>> {
    return this.http.get<ApiResponse<Patient>>(`${this.baseUrl}/${id}`);
  }

  create(patient: CreatePatientRequest): Observable<ApiResponse<Patient>> {
    return this.http.post<ApiResponse<Patient>>(this.baseUrl, patient);
  }

  update(id: string, patient: CreatePatientRequest): Observable<ApiResponse<Patient>> {
    return this.http.put<ApiResponse<Patient>>(`${this.baseUrl}/${id}`, patient);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }

  searchByName(firstName: string, lastName: string): Observable<ApiResponse<Patient[]>> {
    return this.http.get<ApiResponse<Patient[]>>(`${this.baseUrl}/search?firstName=${firstName}&lastName=${lastName}`);
  }
}