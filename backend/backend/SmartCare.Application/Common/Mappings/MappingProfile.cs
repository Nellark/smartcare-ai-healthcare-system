using AutoMapper;
using SmartCare.Application.Common.DTOs;
using SmartCare.Application.Appointments.Commands;
using SmartCare.Application.MedicalRecords.Commands;
using SmartCare.Application.Doctors.Commands;
using SmartCare.Application.Patients.Commands;

namespace SmartCare.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Patient, PatientDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()));

        CreateMap<Domain.Entities.Doctor, DoctorDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<Domain.Entities.MedicalRecord, MedicalRecordDto>();
        CreateMap<Domain.Entities.Appointment, AppointmentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreatePatientCommand, Domain.Entities.Patient>()
            .ConstructUsing(src => ConstructPatient(src))
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<UpdatePatientCommand, Domain.Entities.Patient>()
            .ConstructUsing(src => ConstructUpdatedPatient(src))
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<AddMedicalRecordCommand, Domain.Entities.MedicalRecord>()
            .ConstructUsing(src => ConstructMedicalRecord(src))
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<CreateAppointmentCommand, Domain.Entities.Appointment>()
            .ConstructUsing(src => ConstructAppointment(src))
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<UpdateAppointmentCommand, Domain.Entities.Appointment>()
            .ConstructUsing(src => ConstructAppointment(src))
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<CreateMedicalRecordCommand, Domain.Entities.MedicalRecord>()
            .ConstructUsing(src => ConstructMedicalRecord(src))
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<UpdateMedicalRecordCommand, Domain.Entities.MedicalRecord>()
            .ConstructUsing(src => ConstructMedicalRecord(src))
            .ForAllMembers(opts => opts.Ignore());
    }

    private static Domain.Entities.Patient ConstructPatient(CreatePatientCommand src)
    {
        // This is handled in the command handler, but AutoMapper needs this
        throw new InvalidOperationException("Patient construction is handled in command handler");
    }

    private static Domain.Entities.Patient ConstructUpdatedPatient(UpdatePatientCommand src)
    {
        throw new InvalidOperationException("Patient update is handled in command handler");
    }

    private static Domain.Entities.MedicalRecord ConstructMedicalRecord(AddMedicalRecordCommand src)
    {
        throw new InvalidOperationException("Medical record creation is handled in command handler");
    }

    private static Domain.Entities.Appointment ConstructAppointment(CreateAppointmentCommand src)
    {
        throw new InvalidOperationException("Appointment construction is handled in command handler");
    }

    private static Domain.Entities.Appointment ConstructAppointment(UpdateAppointmentCommand src)
    {
        throw new InvalidOperationException("Appointment construction is handled in command handler");
    }

    private static Domain.Entities.MedicalRecord ConstructMedicalRecord(CreateMedicalRecordCommand src)
    {
        throw new InvalidOperationException("Medical record construction is handled in command handler");
    }

    private static Domain.Entities.MedicalRecord ConstructMedicalRecord(UpdateMedicalRecordCommand src)
    {
        throw new InvalidOperationException("Medical record construction is handled in command handler");
    }
}
