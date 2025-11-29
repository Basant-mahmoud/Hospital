using AutoMapper;
using Hospital.Application.DTO.MedicalRecord;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ILogger<MedicalRecordService> _logger;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepo,
            IDoctorService doctorService,
            IPatientService patientService,
            IMapper mapper,
            IBranchService branchService,
            IAppointmentService appointmentService,
            IDoctorRepository doctorRepoy,
            ILogger<MedicalRecordService> logger)
        {
            _medicalRecordRepo = medicalRecordRepo;
            _doctorService = doctorService;
            _patientService = patientService;
            _mapper = mapper;
            _branchService = branchService;
            _appointmentService = appointmentService;
            _doctorRepoy = doctorRepoy;
            _logger = logger;
        }

        public async Task<MedicalRecordDto> AddAsync(AddMedicalRecordDto dto)
        {
            _logger.LogInformation("Adding medical record for DoctorId {DoctorId}, PatientId {PatientId}, AppointmentId {AppointmentId}",
                dto.DoctorId, dto.PatientId, dto.AppointmentId);

            var doctor = await _doctorService.GetAsync(new() { DoctorId = dto.DoctorId });
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with Id {DoctorId} not found", dto.DoctorId);
                throw new KeyNotFoundException($"Doctor with Id {dto.DoctorId} not found");
            }

            var patient = await _patientService.GetPatientByIdAsync(dto.PatientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient with Id {PatientId} not found", dto.PatientId);
                throw new KeyNotFoundException($"Patient with Id {dto.PatientId} not found");
            }

            var appointment = await _appointmentService.GetByIdwithdetailsofpatientanddoctorAsync(dto.AppointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with Id {AppointmentId} not found", dto.AppointmentId);
                throw new KeyNotFoundException($"Appointment with Id {dto.AppointmentId} not found");
            }
            if (dto.PatientId != appointment.patientInfo.PatientId)
            {
                _logger.LogWarning("patient with Id {PatientId} not found", dto.PatientId);
                throw new KeyNotFoundException($"patient with Id {{PatientId}} not found\", dto.PatientId");
            }
            if (dto.DoctorId!=appointment.doctorinfo.DoctorId)
            {
                _logger.LogWarning("Doctor with Id {DoctorId} not found", dto.DoctorId);
                throw new KeyNotFoundException($"Doctor with Id {{DoctorId}} not found\", dto.DoctorId");
            }

            var medicalRecord = _mapper.Map<MedicalRecord>(dto);
            medicalRecord.CreatedAt = DateTime.UtcNow;
            medicalRecord.UpdatedAt = DateTime.UtcNow;

            await _medicalRecordRepo.AddAsync(medicalRecord);

            var fullRecord = await _medicalRecordRepo.GetByIdWithRelationsAsync(medicalRecord.RecordId);

            _logger.LogInformation("Medical record added successfully with RecordId {RecordId}", fullRecord.RecordId);

            return _mapper.Map<MedicalRecordDto>(fullRecord);
        }

        public async Task<int> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting medical record with RecordId {RecordId}", id);

            var record = await _medicalRecordRepo.GetAsync(id);
            if (record == null)
            {
                _logger.LogWarning("MedicalRecord with Id {RecordId} not found", id);
                throw new KeyNotFoundException($"MedicalRecord with Id {id} not found");
            }

            var result = await _medicalRecordRepo.DeleteAsync(record);
            _logger.LogInformation("MedicalRecord with Id {RecordId} deleted successfully", id);
            return result;
        }

        public async Task<MedicalRecordDto?> GetByMedicalRecordIdAsync(GetMedicalRecordDto dto)
        {
            _logger.LogInformation("Fetching medical record with RecordId {RecordId}", dto.MedicalRecordId);

            var record = await _medicalRecordRepo.GetAsync(dto.MedicalRecordId);
            if (record == null)
            {
                _logger.LogWarning("MedicalRecord with Id {RecordId} not found", dto.MedicalRecordId);
                throw new KeyNotFoundException($"MedicalRecord with Id {dto.MedicalRecordId} not found");
            }

            return _mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<MedicalRecordDto> UpdateAsync(UpdateMedicalRecordDto dto)
        {
            _logger.LogInformation("Updating medical record with RecordId {RecordId}", dto.RecordId);

            var doctor = await _doctorService.GetAsync(new() { DoctorId = dto.DoctorId });
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with Id {DoctorId} not found", dto.DoctorId);
                throw new KeyNotFoundException($"Doctor with Id {dto.DoctorId} not found");
            }

            var patient = await _patientService.GetPatientByIdAsync(dto.PatientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient with Id {PatientId} not found", dto.PatientId);
                throw new KeyNotFoundException($"Patient with Id {dto.PatientId} not found");
            }

            var appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with Id {AppointmentId} not found", dto.AppointmentId);
                throw new KeyNotFoundException($"Appointment with Id {dto.AppointmentId} not found");
            }

            var record = await _medicalRecordRepo.GetAsync(dto.RecordId);
            if (record == null)
            {
                _logger.LogWarning("MedicalRecord with Id {RecordId} not found", dto.RecordId);
                throw new KeyNotFoundException($"MedicalRecord with Id {dto.RecordId} not found");
            }

            _mapper.Map(dto, record);
            record.UpdatedAt = DateTime.UtcNow;

            var result = await _medicalRecordRepo.UpdateAsync(record);
            _logger.LogInformation("MedicalRecord with RecordId {RecordId} updated successfully", dto.RecordId);

            return _mapper.Map<MedicalRecordDto>(result);
        }

        public async Task<List<MedicalRecordDto>> GetByDoctorId(int doctorId)
        {
            _logger.LogInformation("Fetching medical records for DoctorId {DoctorId}", doctorId);

            var doctor = await _doctorRepoy.GetAsync(doctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with Id {DoctorId} not found", doctorId);
                throw new KeyNotFoundException($"Doctor with Id {doctorId} not found");
            }

            var records = await _medicalRecordRepo.GetByDoctorIdAsync(doctorId);
            return _mapper.Map<List<MedicalRecordDto>>(records);
        }

        public async Task<List<MedicalRecordDto>> GetByPatientId(int patientId)
        {
            _logger.LogInformation("Fetching medical records for PatientId {PatientId}", patientId);

            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient with Id {PatientId} not found", patientId);
                throw new KeyNotFoundException($"Patient with Id {patientId} not found");
            }

            var records = await _medicalRecordRepo.GetByPatientIdAsync(patientId);
            return _mapper.Map<List<MedicalRecordDto>>(records);
        }

        public async Task<List<MedicalRecordDto>> GetRecordsBetweenDoctorAndPatientAsync(int doctorId, int patientId)
        {
            _logger.LogInformation("Fetching medical records between DoctorId {DoctorId} and PatientId {PatientId}", doctorId, patientId);

            if (doctorId == 0)
            {
                _logger.LogWarning("Invalid DoctorId {DoctorId}", doctorId);
                throw new ArgumentException("Doctor Id Not Valid");
            }

            var doctor = await _doctorRepoy.GetAsync(doctorId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with Id {DoctorId} not found", doctorId);
                throw new KeyNotFoundException($"Doctor with Id {doctorId} not found");
            }

            if (patientId == 0)
            {
                _logger.LogWarning("Invalid PatientId {PatientId}", patientId);
                throw new ArgumentException("Patient Id Not Valid");
            }

            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient with Id {PatientId} not found", patientId);
                throw new KeyNotFoundException($"Patient with Id {patientId} not found");
            }

            var records = await _medicalRecordRepo.GetByDoctorAndPatientAsync(doctorId, patientId);
            if (records == null || !records.Any())
            {
                _logger.LogInformation("No medical records found between DoctorId {DoctorId} and PatientId {PatientId}", doctorId, patientId);
                return new List<MedicalRecordDto>();
            }

            return _mapper.Map<List<MedicalRecordDto>>(records);
        }
    }
}
