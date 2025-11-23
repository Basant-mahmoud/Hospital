using AutoMapper;
using Hospital.Application.DTO.Appointment;
using Hospital.Application.DTO.MedicalRecord;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class MedicalRecordProfile:Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<AddMedicalRecordDto, MedicalRecord>();
            CreateMap<UpdateMedicalRecordDto, MedicalRecord>();

            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.Appointment, opt => opt.MapFrom(src => src.Appointment));

            CreateMap<Patient, PatientMedicalRecordDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));

            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => new BranchShortDto
                {
                    BranchId = src.Branch.BranchId,
                    Name = src.Branch.BranchName
                }));

            CreateMap<Doctor, DoctorMedicalDto>()
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
              .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));

        }
    }
}
