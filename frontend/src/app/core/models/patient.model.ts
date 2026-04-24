export interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  dateOfBirth: string;
  phoneNumber: string;
  address: string;
  age?: number;
  createdAt?: string;
  updatedAt?: string;
  medicalRecords?: MedicalRecord[];
}

export interface MedicalRecord {
  id: string;
  diagnosis: string;
  treatment: string;
  notes: string;
  recordDate: string;
  doctorId: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface CreatePatientRequest {
  firstName: string;
  lastName: string;
  email: string;
  dateOfBirth: string;
  phoneNumber: string;
  address: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
  timestamp: string;
}