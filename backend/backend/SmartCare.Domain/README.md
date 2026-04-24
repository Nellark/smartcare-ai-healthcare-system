# SmartCare.Domain Layer

This layer implements Domain-Driven Design (DDD) principles with Clean Architecture patterns for the SmartCare healthcare system.

## Architecture Overview

### Core Concepts

#### 1. **Aggregate Root: Patient**
The `Patient` aggregate root ensures business rules are enforced and maintains consistency within its boundary.

**Key Features:**
- **Encapsulated MedicalRecords Collection**: The `MedicalRecords` collection is private and can only be modified through domain methods
- **Invariant Enforcement**: All business rules are enforced within the aggregate
- **Domain Events**: Emits events for important state changes
- **Rich Domain Model**: Contains behavior, not just data

**Domain Methods:**
```csharp
// Create new patient
Result<Patient> Create(...)

// Medical Record Operations (Encapsulated)
Result<MedicalRecord> AddMedicalRecord(...)
Result<MedicalRecord> UpdateMedicalRecord(...)
Result RemoveMedicalRecord(...)
Result<MedicalRecord> GetMedicalRecord(...)

// Personal Information Updates
Result UpdatePersonalInformation(...)

// Query Methods
IReadOnlyCollection<MedicalRecord> GetMedicalRecordsByDateRange(...)
int GetAge()
```

#### 2. **Entity: MedicalRecord**
Part of the Patient aggregate, representing medical records that cannot exist independently.

**Key Features:**
- **Identity**: Has its own `MedicalRecordId`
- **Lifecycle Management**: Created, updated, and removed only through Patient aggregate
- **Business Rules**: Enforces validation for diagnosis, treatment, and doctor ID
- **Audit Trail**: Tracks creation and modification timestamps

#### 3. **Value Objects**
Immutable objects that define concepts without identity.

**Implemented Value Objects:**
- `PatientId`: Strongly-typed identifier for patients
- `MedicalRecordId`: Strongly-typed identifier for medical records  
- `Email`: Validated email address with proper format
- `FullName`: Combined first and last name with validation

#### 4. **Domain Events**
Events that represent something that happened in the domain that domain experts care about.

**Events:**
- `PatientCreatedEvent`: When a new patient is registered
- `MedicalRecordAddedEvent`: When a medical record is added
- `MedicalRecordUpdatedEvent`: When a medical record is modified
- `MedicalRecordRemovedEvent`: When a medical record is deleted
- `PatientEmailUpdatedEvent`: When patient email changes

#### 5. **Repository Interfaces**
Abstract data access contracts that follow the Repository pattern.

**Repositories:**
- `IPatientRepository`: Patient-specific operations
- `IRepository<T, TId>`: Generic repository interface

#### 6. **Specifications**
Encapsulated query logic that can be combined and reused.

**Specifications:**
- `PatientById`: Find patient by ID
- `PatientByEmail`: Find patient by email
- `PatientsOlderThan`: Filter patients by age
- `PatientsWithMedicalRecordsInDateRange`: Complex query with date filtering

#### 7. **Domain Services**
Services that encapsulate business logic that doesn't naturally fit within a single aggregate.

**Services:**
- `IPatientDomainService`: Complex patient-related business operations

## Key DDD Principles Applied

### 1. **Ubiquitous Language**
- All code uses healthcare domain terminology
- Patient, MedicalRecord, Diagnosis, Treatment are first-class concepts
- Business rules are expressed in code

### 2. **Bounded Context**
- Clear separation between Patient management and other domains
- Well-defined aggregate boundaries
- Explicit invariants and business rules

### 3. **Aggregate Design**
- **Patient** is the aggregate root
- **MedicalRecord** is an entity within the Patient aggregate
- All access to MedicalRecords goes through Patient
- Consistency boundaries are clearly defined

### 4. **Encapsulation**
- `MedicalRecords` collection is private
- Only domain methods can modify the collection
- External code cannot directly manipulate the collection
- All modifications go through validated methods

### 5. **Rich Domain Model**
- Entities contain behavior, not just data
- Business logic is encapsulated within the domain
- Validation and rules are enforced at the domain level
- Result pattern for error handling

## Usage Examples

### Creating a Patient
```csharp
var patientId = PatientId.Create();
var name = FullName.Create("John", "Doe").Value;
var email = Email.Create("john.doe@example.com").Value;

var result = Patient.Create(
    patientId,
    name,
    email,
    new DateTime(1980, 1, 1),
    "555-1234",
    "123 Main St"
);

if (result.IsSuccess)
{
    var patient = result.Value;
    // Patient created successfully
}
```

### Adding a Medical Record (Encapsulated)
```csharp
var medicalRecordId = MedicalRecordId.Create();

var result = patient.AddMedicalRecord(
    medicalRecordId,
    "Hypertension",
    "Prescribed ACE inhibitors",
    "Patient shows elevated blood pressure",
    DateTime.UtcNow,
    "doctor-123"
);

if (result.IsSuccess)
{
    var medicalRecord = result.Value;
    // Medical record added successfully
}
```

### Querying Medical Records
```csharp
// Get all medical records in date range
var records = patient.GetMedicalRecordsByDateRange(
    startDate,
    endDate
);

// Get specific medical record
var result = patient.GetMedicalRecord(medicalRecordId);
```

## Benefits of This Design

1. **Encapsulation**: MedicalRecords collection is fully encapsulated
2. **Business Rule Enforcement**: All validation happens in the domain
3. **Testability**: Pure domain logic without infrastructure dependencies
4. **Maintainability**: Clear separation of concerns
5. **Scalability**: Domain logic is isolated and can evolve independently
6. **Event-Driven**: Domain events enable loose coupling with other systems

## Next Steps

1. **Implementation Layer**: Create Application Services to coordinate use cases
2. **Infrastructure Layer**: Implement repositories with Entity Framework
3. **API Layer**: Expose domain operations through RESTful endpoints
4. **Testing**: Unit tests for domain logic and business rules
5. **Validation**: Comprehensive input validation and error handling
