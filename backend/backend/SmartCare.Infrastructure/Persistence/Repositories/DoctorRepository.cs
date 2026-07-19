using Microsoft.EntityFrameworkCore;
using SmartCare.Domain.Entities;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;
using SmartCare.Infrastructure.Persistence.Entities;

namespace SmartCare.Infrastructure.Persistence.Repositories;

public sealed class DoctorRepository : IDoctorRepository
{
    private readonly SmartCareDbContext _context;

    public DoctorRepository(SmartCareDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor?> GetByIdAsync(DoctorId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Doctors.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null ? null : MapToDomainEntity(entity);
    }

    public async Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Doctors.ToListAsync(cancellationToken);
        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Doctor>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Doctors.Where(x => x.Email == email.Value).ToListAsync(cancellationToken);
        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Doctor>> GetByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Doctors
            .Where(x => x.FirstName == firstName && x.LastName == lastName)
            .ToListAsync(cancellationToken);
        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Doctor>> GetBySpecialtyAsync(string specialty, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Doctors
            .Where(x => x.Specialty == specialty)
            .ToListAsync(cancellationToken);
        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public Task<bool> ExistsAsync(DoctorId id, CancellationToken cancellationToken = default)
        => _context.Doctors.AnyAsync(x => x.Id == id, cancellationToken);

    public Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
        => _context.Doctors.AnyAsync(x => x.Email == email.Value, cancellationToken);

    public Task<bool> LicenseNumberExistsAsync(string licenseNumber, CancellationToken cancellationToken = default)
        => _context.Doctors.AnyAsync(x => x.LicenseNumber == licenseNumber, cancellationToken);

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        await _context.Doctors.AddAsync(MapToInfrastructureEntity(doctor), cancellationToken);
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Doctors.FirstOrDefaultAsync(x => x.Id == doctor.Id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.FirstName = doctor.Name.FirstName;
        entity.LastName = doctor.Name.LastName;
        entity.Email = doctor.Email;
        entity.Specialty = doctor.Specialty;
        entity.PhoneNumber = doctor.PhoneNumber;
        entity.LicenseNumber = doctor.LicenseNumber;
        entity.UpdatedAt = doctor.UpdatedAt;
    }

    public async Task DeleteAsync(DoctorId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Doctors.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Doctors.Remove(entity);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    private static Doctor MapToDomainEntity(DoctorEntity entity)
    {
        var nameResult = FullName.Create(entity.FirstName, entity.LastName);
        var emailResult = Email.Create(entity.Email);

        if (!nameResult.IsSuccess || !emailResult.IsSuccess)
        {
            throw new InvalidOperationException("Invalid entity data");
        }

        return Doctor.Rehydrate(
            entity.Id,
            nameResult.Value,
            emailResult.Value,
            entity.Specialty,
            entity.PhoneNumber,
            entity.LicenseNumber,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    private static DoctorEntity MapToInfrastructureEntity(Doctor doctor)
    {
        return new DoctorEntity
        {
            Id = doctor.Id,
            FirstName = doctor.Name.FirstName,
            LastName = doctor.Name.LastName,
            Email = doctor.Email,
            Specialty = doctor.Specialty,
            PhoneNumber = doctor.PhoneNumber,
            LicenseNumber = doctor.LicenseNumber,
            CreatedAt = doctor.CreatedAt,
            UpdatedAt = doctor.UpdatedAt
        };
    }
}
