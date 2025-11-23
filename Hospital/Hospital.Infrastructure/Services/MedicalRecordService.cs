using AutoMapper;
using Hospital.Application.DTO.MedicalRecord;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
     public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepo;
        private readonly IDoctorService _doctorService;
        private readonly IDoctorRepository _doctorRepoy;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IBranchService _branchService;
        private readonly IMapper _mapper;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepo,
            IDoctorService doctorService,
            IPatientService patientService,
            IMapper mapper,
            IBranchService branchService,
            IAppointmentService appointmentService,
            IDoctorRepository doctorRepoy)
        {
            _medicalRecordRepo = medicalRecordRepo;
            _doctorService = doctorService;
            _patientService = patientService;
            _mapper = mapper;
            _branchService = branchService;
            _appointmentService = appointmentService;
            _doctorRepoy = doctorRepoy;
        }


        public async Task<MedicalRecordDto> AddAsync(AddMedicalRecordDto dto)
        {
            var doctor = await _doctorService.GetAsync(new() { DoctorId = dto.DoctorId });
            if (doctor == null)
                throw new Exception($"Doctor with Id {dto.DoctorId} not found");

            var patient = await _patientService.GetPatientByIdAsync(dto.PatientId);
            if (patient == null)
                throw new Exception($"Patient with Id {dto.PatientId} not found");

            var appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
           if (appointment == null)
              throw new Exception($"Appointment with Id {dto.AppointmentId} not found");

            var medicalRecord = _mapper.Map<MedicalRecord>(dto);
            medicalRecord.CreatedAt = DateTime.UtcNow;
            medicalRecord.UpdatedAt = DateTime.UtcNow;

            await _medicalRecordRepo.AddAsync(medicalRecord);

            var fullRecord = await _medicalRecordRepo.GetByIdWithRelationsAsync(medicalRecord.RecordId);

            return _mapper.Map<MedicalRecordDto>(fullRecord);
        }


        public async Task<int> DeleteAsync(int id)
        {
            var record = await _medicalRecordRepo.GetAsync(id);
            if (record == null) throw new Exception($"MedicalRecord with Id {id} not found");
            return await _medicalRecordRepo.DeleteAsync(record);
        }

      
        public async Task<MedicalRecordDto?> GetByMedicalRecordIdAsync(GetMedicalRecordDto dto)
        {

            var record = await _medicalRecordRepo.GetAsync(dto.MedicalRecordId);
            if (record == null) return null;
            return _mapper.Map<MedicalRecordDto>(record);
        }

       
        public async Task<int> UpdateAsync(UpdateMedicalRecordDto dto)
        {
            var doctor = await _doctorService.GetAsync(new() { DoctorId = dto.DoctorId });
            if (doctor == null)
                throw new Exception($"Doctor with Id {dto.DoctorId} not found");

            var patient = await _patientService.GetPatientByIdAsync(dto.PatientId);
            if (patient == null)
                throw new Exception($"Patient with Id {dto.PatientId} not found");

            var appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
                throw new Exception($"Appointment with Id {dto.AppointmentId} not found");

            var record = await _medicalRecordRepo.GetAsync(dto.RecordId);
            if (record == null) throw new Exception($"MedicalRecord with Id {dto.RecordId} not found");

            _mapper.Map(dto, record); 
            record.UpdatedAt = DateTime.UtcNow;

            return await _medicalRecordRepo.UpdateAsync(record);
        }
        public async Task<List<MedicalRecordDto>> GetByDoctorId(int doctorId)
        {
            
            var doctor = await _doctorRepoy.GetAsync(doctorId);
            if (doctor == null)
                throw new Exception($"Doctor with Id {doctorId} not found");
            var records = await _medicalRecordRepo.GetByDoctorIdAsync(doctorId);
            return _mapper.Map<List<MedicalRecordDto>>(records);
        }

        public async Task<List<MedicalRecordDto>> GetByPatientId(int patientId)
        {
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
                throw new Exception($"Patient with Id {patientId} not found");
            var records = await _medicalRecordRepo.GetByPatientIdAsync(patientId);
            return _mapper.Map<List<MedicalRecordDto>>(records);
        }

        public async Task<List<MedicalRecordDto>> GetRecordsBetweenDoctorAndPatientAsync(int doctorId, int patientId)
        {
            if (doctorId == 0)
                throw new Exception("Doctor Id Not Valied");
            // Check doctor
            var doctor = await _doctorRepoy.GetAsync(doctorId);
            if (doctor == null)
                throw new Exception($"Doctor with Id {doctorId} not found");
            if (doctorId == 0)
                throw new Exception("Patient Id Not Valied");
            // Check patient
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
                throw new Exception($"Patient with Id {patientId} not found");

            // Fetch record
            var records = await _medicalRecordRepo.GetByDoctorAndPatientAsync(doctorId, patientId);

            if (records == null || !records.Any())
                return new List<MedicalRecordDto>(); 

            return _mapper.Map<List<MedicalRecordDto>>(records);
        }


    }
}
