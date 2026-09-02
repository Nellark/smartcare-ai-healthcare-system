using Microsoft.EntityFrameworkCore;
using SmartCare.Domain.ValueObjects;
using SmartCare.Infrastructure.Persistence.Entities;

namespace SmartCare.Infrastructure.Persistence;

public class SmartCareDbContext : DbContext
{
    public SmartCareDbContext(DbContextOptions<SmartCareDbContext> options) : base(options)
    {
    }

    public DbSet<PatientEntity> Patients { get; set; }
    public DbSet<MedicalRecordEntity> MedicalRecords { get; set; }
    public DbSet<DoctorEntity> Doctors { get; set; }
    public DbSet<AppointmentEntity> Appointments { get; set; }
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartCareDbContext).Assembly);

        // Configure relationships and constraints
        ConfigurePatientEntity(modelBuilder);
        ConfigureMedicalRecordEntity(modelBuilder);
        ConfigureDoctorEntity(modelBuilder);
        ConfigureAppointmentEntity(modelBuilder);
        ConfigureUserEntity(modelBuilder);
    }

    private static void ConfigurePatientEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasConversion(id => id.Value, value => PatientId.FromGuid(value));

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => new { e.FirstName, e.LastName });
        });
    }

    private static void ConfigureUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
        });
    }

    private static void ConfigureMedicalRecordEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalRecordEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasConversion(id => id.Value, value => MedicalRecordId.FromGuid(value));

            entity.Property(e => e.Diagnosis)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.Treatment)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.Notes)
                .HasMaxLength(2000);

            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.RecordDate)
                .IsRequired();

            entity.Property(e => e.RecordType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Provider)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.AttachmentUrl)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt);

            // Relationship with Patient
            entity.HasOne<PatientEntity>()
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DoctorId);
            entity.HasIndex(e => e.RecordDate);
        });
    }

    private static void ConfigureDoctorEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoctorEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasConversion(id => id.Value, value => DoctorId.FromGuid(value));

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Specialty)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
            entity.HasIndex(e => e.Specialty);
            entity.HasIndex(e => new { e.FirstName, e.LastName });
        });
    }

    private static void ConfigureAppointmentEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasConversion(id => id.Value, value => AppointmentId.FromGuid(value));

            entity.Property(e => e.ScheduledAt).IsRequired();
            entity.Property(e => e.DurationMinutes).IsRequired();
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.HasOne(e => e.Doctor)
                .WithMany()
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DoctorId);
            entity.HasIndex(e => e.ScheduledAt);
            entity.HasIndex(e => new { e.PatientId, e.ScheduledAt });
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditableEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                
                // Prevent modification of CreatedAt
                entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
            }
        }
    }
}
