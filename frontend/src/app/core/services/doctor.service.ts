import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Doctor, UpsertDoctorRequest } from '../models/doctor.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private readonly baseUrl = `${environment.apiBaseUrl}/doctors`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Doctor[]>> {
    return this.http.get<ApiResponse<Doctor[]>>(this.baseUrl);
  }

  getById(id: string): Observable<ApiResponse<Doctor>> {
    return this.http.get<ApiResponse<Doctor>>(`${this.baseUrl}/${id}`);
  }

  searchBySpecialty(specialty: string): Observable<ApiResponse<Doctor[]>> {
    const params = new HttpParams().set('specialty', specialty);
    return this.http.get<ApiResponse<Doctor[]>>(`${this.baseUrl}/search`, { params });
  }

  create(doctor: UpsertDoctorRequest): Observable<ApiResponse<Doctor>> {
    return this.http.post<ApiResponse<Doctor>>(this.baseUrl, doctor);
  }

  update(id: string, doctor: UpsertDoctorRequest): Observable<ApiResponse<Doctor>> {
    return this.http.put<ApiResponse<Doctor>>(`${this.baseUrl}/${id}`, doctor);
  }

  delete(id: string): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.baseUrl}/${id}`);
  }
}
