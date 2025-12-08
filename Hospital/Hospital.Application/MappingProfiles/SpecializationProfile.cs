using AutoMapper;
using Hospital.Application.DTO.Specialization;
using Hospital.Domain.Models;

namespace Hospital.Application.MappingProfiles
{
    public class SpecializationProfile : Profile
    {
        public SpecializationProfile()
        {
            // CreateSpecialization → Specialization
            CreateMap<CreateSpecialization, Specialization>()
                .ForMember(dest => dest.SpecializationId, opt => opt.Ignore())
                .ForMember(dest => dest.Branches, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // UpdateSpecialization → Specialization
            CreateMap<UpdateSpecialization, Specialization>()
                .ForMember(dest => dest.Branches, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Specialization → SpecializationDTO
            CreateMap<Specialization, SpecializationDTO>()
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
                .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
                .ForMember(dest => dest.Branches, opt => opt.MapFrom(src => src.Branches))
                .ReverseMap();

            // Specialization → SpecializationInfoDto
            CreateMap<Specialization, SpecializationInfoDto>()
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
                .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.Doctors))
                .ForMember(dest => dest.Branches, opt => opt.MapFrom(src => src.Branches));

            // Doctor → DoctorMiniDto
            CreateMap<Doctor, DoctorMiniDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));

            // Branch → BranchMiniDto
            CreateMap<Branch, BranchMiniDto>();
        }
    }
}
