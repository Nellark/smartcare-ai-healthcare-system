export interface MedicalRecord {
  id: string;
  patientId: string;
  diagnosis: string;
  treatment: string;
  notes: string;
  recordDate: string;
  doctorId: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface UpsertMedicalRecordRequest {
  patientId: string;
  diagnosis: string;
  treatment: string;
  notes: string;
  recordDate: string;
  doctorId: string;
}
