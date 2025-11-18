using AutoMapper;
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
            CreateMap<MedicalRecord, PatientMedicalRecordDto>().ReverseMap();
            CreateMap<AddMedicalRecordDto, MedicalRecord>();
            CreateMap<MedicalRecord, PatientMedicalRecordDto>()
    .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.FullName))
    .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.User.FullName));

        }
    }
}
