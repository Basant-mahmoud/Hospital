using AutoMapper;
using Hospital.Application.DTO.Appointment;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IBranchRepository _branchRepo;

        public AppointmentService(IAppointmentRepository appointmentRepo, IPatientRepository patientRepo, IDoctorRepository doctorRepository, IBranchRepository branchRepo, IMapper mapper)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _doctorRepository = doctorRepository;
            _branchRepo = branchRepo;
            _mapper = mapper;
        }

        public async Task<AppointmentDto> AddAsync(AddAppointmentDto dto)
        {
            var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
            if (patient == null)
                throw new ArgumentException("Patient does not exist.");

            var doctor = await _doctorRepository.GetAsync(dto.DoctorId);
            if (doctor == null)
                throw new ArgumentException("Doctor does not exist.");

            var branch = await _branchRepo.GetByIdAsync(dto.BranchId);
            if (branch == null)
                throw new ArgumentException("Branch does not exist.");


            if (dto.PaymentMethod != PaymentMethod.Cash &&
                dto.PaymentMethod != PaymentMethod.Paymob)
            {
                throw new ArgumentException("Invalid payment method. Allowed: Cash or Paymob");
            }

            if (dto.Date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new ArgumentException("Appointment date cannot be in the past.");

            if (dto.Time.Hour < 8 || dto.Time.Hour > 20)
                throw new ArgumentException("Appointment time must be between 08:00 and 20:00.");

            // Prevent duplicate appointment (doctor same date/time)
            var exists = await _appointmentRepo.ExistsAsync(dto.DoctorId, dto.Date, dto.Time);
            if (exists)
                throw new ArgumentException("Doctor already has an appointment at this time.");

            var appointment = _mapper.Map<Appointment>(dto);
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;
            appointment.Status = AppointmentStatus.Confirmed;

            var created = await _appointmentRepo.AddAsync(appointment);

            return _mapper.Map<AppointmentDto>(created);
        }


        public async Task<int> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid appointment ID.");

            var record = await _appointmentRepo.GetAsync(id);
            if (record == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");

            return await _appointmentRepo.DeleteAsync(record);
        }

        public async Task<List<AppointmentDto>> GetByDoctorId(int DoctorId)
        {
            if (DoctorId <= 0)
                throw new ArgumentException("Invalid doctor ID.");

            var doctor = await _doctorRepository.GetAsync(DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {DoctorId} does not exist.");

            var records = await _appointmentRepo.GetByDoctorIdAsync(DoctorId);

            if (records == null || !records.Any())
                return new List<AppointmentDto>(); 

            return _mapper.Map<List<AppointmentDto>>(records);
        }

        public async Task<AppointmentDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid appointment ID.");

            var record = await _appointmentRepo.GetAsync(id);
            if (record == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");

            return _mapper.Map<AppointmentDto>(record);
        }

        public async Task<List<AppointmentDto>> GetByPatientId(int PatientId)
        {
            if (PatientId <= 0)
                throw new ArgumentException("Invalid patient ID.");

            var patient = await _patientRepo.GetByIdAsync(PatientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {PatientId} does not exist.");

            var records = await _appointmentRepo.GetByPatientIdAsync(PatientId);

            if (records == null || !records.Any())
                return new List<AppointmentDto>();

            return _mapper.Map<List<AppointmentDto>>(records);
        }
    }
}
