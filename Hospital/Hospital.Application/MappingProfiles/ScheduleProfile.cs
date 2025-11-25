using AutoMapper;
using Hospital.Application.DTO.Schedule;
using Hospital.Domain.Models;

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
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                .ForMember(dest => dest.AppointmentShift, opt => opt.MapFrom(src => src.Shift));
        }
    }
}
