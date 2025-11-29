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
                // Schedule Info
                .ForMember(dest => dest.AppointmentShift, opt => opt.MapFrom(src => src.Shift))

                // Doctor Info
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.FullName))
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.Doctor.ImageURL))
                .ForMember(dest => dest.ConsultationFees, opt => opt.MapFrom(src => src.Doctor.ConsultationFees))
                .ForMember(dest => dest.Available, opt => opt.MapFrom(src => src.Doctor.Available))

                // Specialization Info
                .ForMember(dest => dest.SpecializationId, opt => opt.MapFrom(src => src.Doctor.SpecializationId))
                .ForMember(dest => dest.SpecializationName, opt => opt.MapFrom(src => src.Doctor.Specialization.Name))

                // Branch Info
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName));
        }
    }
}
