using AutoMapper;
using Hospital.Application.DTO.Appointment;
using Hospital.Domain.Models;

namespace Hospital.Application.MappingProfiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            // AddAppointmentDto → Appointment
            CreateMap<AddAppointmentDto, Appointment>()
                .ForMember(dest => dest.AppointmentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Appointment → AppointmentDto
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString())) // Enum → String
               .ForMember(dest => dest.Shift,  // Map Shift enum to string
                    opt => opt.MapFrom(src => src.Shift.ToString()));

            // Branch → BranchShortDto
            CreateMap<Branch, BranchShortDto>();

            // Patient → PatientAppointmentDto
            CreateMap<Patient, PatientAppointmentDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));

            // Appointment → AppoinmentandPaientDetaliesDto
            CreateMap<Appointment, AppoinmentandPaientDetaliesDto>()
                .ForMember(dest => dest.appointmentDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.patientInfo, opt => opt.MapFrom(src => src.Patient));
            // Doctor → appointmentDoctorDto
            CreateMap<Doctor, appointmentDoctorDto>()
                .ForMember(dest => dest.SpecializationId, opt => opt.MapFrom(src => src.SpecializationId));

            // Appointment → AppoinmentandPaientDoctorDetaliesDto
            CreateMap<Appointment, AppoinmentandPaientDoctorDetaliesDto>()
                .ForMember(dest => dest.appointmentDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.patientInfo, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.doctorinfo, opt => opt.MapFrom(src => src.Doctor));

        }
    }
}
