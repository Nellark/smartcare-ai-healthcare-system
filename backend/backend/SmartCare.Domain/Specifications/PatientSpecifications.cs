namespace SmartCare.Domain.Specifications;

public static class PatientSpecifications
{
    public sealed class PatientById : ISpecification<Patient>
    {
        private readonly PatientId _id;

        public PatientById(PatientId id)
        {
            _id = id;
        }

        public Expression<Func<Patient, bool>> ToExpression()
            => patient => patient.Id == _id;

        public Func<Patient, Patient> Selector => patient => patient;
    }

    public sealed class PatientByEmail : ISpecification<Patient>
    {
        private readonly Email _email;

        public PatientByEmail(Email email)
        {
            _email = email;
        }

        public Expression<Func<Patient, bool>> ToExpression()
            => patient => patient.Email == _email;

        public Func<Patient, Patient> Selector => patient => patient;
    }

    public sealed class PatientsOlderThan : ISpecification<Patient>
    {
        private readonly int _age;

        public PatientsOlderThan(int age)
        {
            _age = age;
        }

        public Expression<Func<Patient, bool>> ToExpression()
            => patient => patient.GetAge() > _age;

        public Func<Patient, Patient> Selector => patient => patient;
    }

    public sealed class PatientsWithMedicalRecordsInDateRange : ISpecification<Patient>
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public PatientsWithMedicalRecordsInDateRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }

        public Expression<Func<Patient, bool>> ToExpression()
            => patient => patient.MedicalRecords.Any(mr => mr.RecordDate >= _startDate && mr.RecordDate <= _endDate);

        public Func<Patient, Patient> Selector => patient => patient;
    }
}
