using AutoMapper;
using Hospital.Application.DTO.Schedule;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<CreateScheduleDto, Schedule>()
                .ForMember(dest => dest.StartTime, opt => opt.Ignore())
                .ForMember(dest => dest.EndTime, opt => opt.Ignore())
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.AppointmentShift));

            CreateMap<UpdateScheduleDto, Schedule>()
                .ForMember(dest => dest.StartTime, opt => opt.Ignore())
                .ForMember(dest => dest.EndTime, opt => opt.Ignore())
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.AppointmentShift));

            CreateMap<Schedule, ScheduleDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.FullName))
                .ForMember(dest => dest.AppointmentShift, opt => opt.MapFrom(src => src.Shift));

        }
    }

}