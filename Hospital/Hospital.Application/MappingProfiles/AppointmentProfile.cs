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

            // Payment → PaymentDto
            CreateMap<Payment, PaymentDto>();

            // Appointment → AppointmentDto 
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.Shift,
                    opt => opt.MapFrom(src => src.Shift.ToString()))
                .ForMember(dest => dest.Payment,         // <<< رجّع البايمنت
                    opt => opt.MapFrom(src => src.Payment));

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

            CreateMap<Doctor, DoctorShortDto>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
    .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization));

            CreateMap<Specialization, SpecializationDto>();
            CreateMap<Appointment, AppointmentDto>();
            // Appointment → GetAllAppointmentCancelDto
            CreateMap<Appointment, GetAllAppointmentCancelDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient));

            // Patient → PatientAppointmentDto
            CreateMap<Patient, PatientAppointmentDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));


        }
    }
}
