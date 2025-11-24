using AutoMapper;
using Hospital.Application.DTO.Patient;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository, IMapper mapper, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            _logger.LogInformation("Fetching patient with ID: {PatientId}", id);
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    throw new KeyNotFoundException($"Patient with ID {id} not found");
                }

                _logger.LogInformation("Patient with ID {PatientId} retrieved successfully", id);
                return _mapper.Map<PatientDto>(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching patient with ID: {PatientId}", id);
                throw;
            }
        }

        public async Task<List<PatientDto>> GetAllPatientsAsync()
        {
            _logger.LogInformation("Fetching all patients");
            try
            {
                var patients = await _patientRepository.GetAllAsync();
                _logger.LogInformation("{Count} patients retrieved successfully", patients?.Count ?? 0);
                return _mapper.Map<List<PatientDto>>(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all patients");
                throw;
            }
        }

        public async Task<PatientDto> UpdatePatientAsync(UpdatePatientDto dto)
        {
            _logger.LogInformation("Updating patient with ID: {PatientId}", dto.PatientId);
            try
            {
                var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", dto.PatientId);
                    throw new KeyNotFoundException($"Patient with ID {dto.PatientId} not found");
                }

                patient.User.FullName = dto.FullName;
                patient.User.PhoneNumber = dto.PhoneNumber;
                patient.User.Email = dto.Email ?? patient.User.Email;
                patient.User.NormalizedEmail = dto.Email?.ToUpper() ?? patient.User.NormalizedEmail;
                patient.User.NormalizedUserName = dto.Email?.ToUpper() ?? patient.User.NormalizedUserName;

                patient.UpdatedAt = DateTime.UtcNow;
                await _patientRepository.UpdateAsync(patient);

                _logger.LogInformation("Patient with ID {PatientId} updated successfully", dto.PatientId);
                return _mapper.Map<PatientDto>(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating patient with ID: {PatientId}", dto.PatientId);
                throw;
            }
        }
    }
}
