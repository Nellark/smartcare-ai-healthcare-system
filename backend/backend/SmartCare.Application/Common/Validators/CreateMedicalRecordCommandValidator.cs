using FluentValidation;
using SmartCare.Application.MedicalRecords.Commands;

namespace SmartCare.Application.Common.Validators;

public class CreateMedicalRecordCommandValidator : AbstractValidator<CreateMedicalRecordCommand>
{
    public CreateMedicalRecordCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.Diagnosis)
            .NotEmpty().WithMessage("Diagnosis is required")
            .MinimumLength(3).WithMessage("Diagnosis must be at least 3 characters");

        RuleFor(x => x.Treatment)
            .NotEmpty().WithMessage("Treatment is required")
            .MinimumLength(3).WithMessage("Treatment must be at least 3 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");

        RuleFor(x => x.RecordDate)
            .NotEmpty().WithMessage("Record date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Record date cannot be in the future");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required");
    }
}

public class UpdateMedicalRecordCommandValidator : AbstractValidator<UpdateMedicalRecordCommand>
{
    public UpdateMedicalRecordCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Medical record ID is required");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.Diagnosis)
            .NotEmpty().WithMessage("Diagnosis is required")
            .MinimumLength(3).WithMessage("Diagnosis must be at least 3 characters");

        RuleFor(x => x.Treatment)
            .NotEmpty().WithMessage("Treatment is required")
            .MinimumLength(3).WithMessage("Treatment must be at least 3 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");

        RuleFor(x => x.RecordDate)
            .NotEmpty().WithMessage("Record date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Record date cannot be in the future");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required");
    }
}
