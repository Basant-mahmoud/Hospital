using AutoMapper;
using Hospital.Application.DTO.News;
using Hospital.Application.DTO.Specialization;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class SpecializationProfile : Profile
    {
        public SpecializationProfile()
        {
            CreateMap<CreateSpecialization, Specialization>()
                .ForMember(dest => dest.SpecializationId, opt => opt.Ignore())
                .ForMember(dest => dest.Branches, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateSpecialization, Specialization>()
                .ForMember(dest => dest.Branches, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Specialization, SpecializationDTO>().ReverseMap();
            CreateMap<Specialization, SpecializationInfoDto>()
             .ForMember(dest => dest.Doctors,
                        opt => opt.MapFrom(src => src.Doctors))
             .ForMember(dest => dest.Branches,
                        opt => opt.MapFrom(src => src.Branches));

            CreateMap<Doctor, DoctorMiniDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<Branch, BranchMiniDto>();
        }

    }
}
