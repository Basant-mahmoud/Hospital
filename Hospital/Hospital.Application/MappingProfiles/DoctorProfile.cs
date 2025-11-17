using AutoMapper;
using Hospital.Application.DTO.Doctor;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<AddDoctorDto, Doctor>()
                 .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                 .ForMember(dest => dest.UserId, opt => opt.Ignore()) // set after register
                 .ForMember(dest => dest.Branches, opt => opt.Ignore())
                 .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                 .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Doctor, DoctorDto>()
      .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName))
      .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User.Email))
      .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User.FullName))
      .ForMember(d => d.SpecializationName, opt => opt.MapFrom(s => s.Specialization != null ? s.Specialization.Name : null))
      .ForMember(d => d.BranchID, opt => opt.MapFrom(s => s.Branches.Select(b => b.BranchId).ToList()));


            CreateMap<DoctorDto, Doctor>()
            .ForMember(dest => dest.DoctorId, opt => opt.Ignore()) // لا تغير الـ PK
            .ForMember(dest => dest.UserId, opt => opt.Ignore())   // لا تغير UserId
            .ForMember(dest => dest.Branches, opt => opt.Ignore()) // لا تغير العلاقات هنا
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // لا تغير CreatedAt
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // UpdatedAt هنديره يدوياً

            CreateMap<DoctorSelfUpdateDto, Doctor>()
    .ForMember(dest => dest.User, opt => opt.Ignore()) // Don't overwrite the User navigation automatically
    .ForMember(dest => dest.Branches, opt => opt.Ignore()); // Handle branches manually if needed


        }
    }
}
