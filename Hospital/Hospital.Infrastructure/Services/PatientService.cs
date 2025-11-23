using AutoMapper;
using Hospital.Application.DTO.Patient;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class PatientService: IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        public PatientService(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {id} not found");

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<List<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            return _mapper.Map<List<PatientDto>>(patients);
        }

        public async Task<PatientDto> UpdatePatientAsync(UpdatePatientDto dto)
        {
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {dto.PatientId} not found");

            patient.User.FullName = dto.FullName;
            patient.User.PhoneNumber = dto.PhoneNumber;
            patient.User.Email = dto.Email ?? patient.User.Email;
            patient.User.NormalizedEmail= dto.Email?.ToUpper() ?? patient.User.NormalizedEmail;
            patient.User.NormalizedUserName= dto.Email?.ToUpper() ?? patient.User.NormalizedUserName;

            patient.UpdatedAt = DateTime.UtcNow;
            await _patientRepository.UpdateAsync(patient);

            return _mapper.Map<PatientDto>(patient);
        }
    }
}

