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

        CreateMap<Domain.Entities.MedicalRecord, MedicalRecordDto>();

        CreateMap<CreatePatientCommand, Domain.Entities.Patient>()
            .ConstructUsing(src => ConstructPatient(src))
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<UpdatePatientCommand, Domain.Entities.Patient>()
            .ConstructUsing(src => null) // Will be handled in command
            .ForAllMembers(opts => opts.Ignore());

        CreateMap<AddMedicalRecordCommand, Domain.Entities.MedicalRecord>()
            .ConstructUsing(src => null) // Will be handled in command
            .ForAllMembers(opts => opts.Ignore());
    }

    private static Domain.Entities.Patient ConstructPatient(CreatePatientCommand src)
    {
        // This is handled in the command handler, but AutoMapper needs this
        throw new InvalidOperationException("Patient construction is handled in command handler");
    }
}
