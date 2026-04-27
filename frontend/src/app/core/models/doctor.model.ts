export interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  specialty: string;
  phoneNumber: string;
  licenseNumber: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface UpsertDoctorRequest {
  firstName: string;
  lastName: string;
  email: string;
  specialty: string;
  phoneNumber: string;
  licenseNumber: string;
}
