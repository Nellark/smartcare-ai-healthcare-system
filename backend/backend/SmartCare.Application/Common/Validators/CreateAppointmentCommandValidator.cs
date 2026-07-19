using FluentValidation;
using SmartCare.Application.Appointments.Commands;

namespace SmartCare.Application.Common.Validators;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required");

        RuleFor(x => x.ScheduledAt)
            .NotEmpty().WithMessage("Scheduled date/time is required");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than zero");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(1000).WithMessage("Reason cannot exceed 1000 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required");
    }
}

public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required");

        RuleFor(x => x.ScheduledAt)
            .NotEmpty().WithMessage("Scheduled date/time is required");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than zero");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(1000).WithMessage("Reason cannot exceed 1000 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required");
    }
}
