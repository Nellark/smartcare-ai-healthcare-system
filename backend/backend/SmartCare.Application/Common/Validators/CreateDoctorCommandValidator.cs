using FluentValidation;
using SmartCare.Application.Doctors.Commands;

namespace SmartCare.Application.Common.Validators;

public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Specialty)
            .NotEmpty().WithMessage("Specialty is required")
            .MaximumLength(100).WithMessage("Specialty cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MinimumLength(10).WithMessage("Phone number must be at least 10 characters");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required")
            .MaximumLength(50).WithMessage("License number cannot exceed 50 characters");
    }
}

public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Doctor ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Specialty)
            .NotEmpty().WithMessage("Specialty is required")
            .MaximumLength(100).WithMessage("Specialty cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MinimumLength(10).WithMessage("Phone number must be at least 10 characters");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required")
            .MaximumLength(50).WithMessage("License number cannot exceed 50 characters");
    }
}
