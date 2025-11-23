using AutoMapper;
using Hospital.Application.DTO.SupportTicket;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly ISupportTicketRepository _repo;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SupportTicketService> _logger;

        public SupportTicketService(ISupportTicketRepository repo, IMapper mapper, IPatientRepository patientRepository, ILogger<SupportTicketService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task<SupportTicketDto> CreateAsync(CreateSupportTicketDto dto)
        {
            _logger.LogInformation("Creating a new support ticket for PatientId {PatientId}", dto.PatientId);

            if (dto == null)
            {
                _logger.LogError("CreateSupportTicketDto is null");
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Subject) || string.IsNullOrWhiteSpace(dto.Message))
            {
                _logger.LogWarning("Subject or Message is empty for PatientId {PatientId}", dto.PatientId);
                throw new ArgumentException("Subject and Message are required.");
            }

            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", dto.PatientId);
                throw new KeyNotFoundException("Patient not found.");
            }

            var ticket = _mapper.Map<SupportTicket>(dto);
            ticket.UserId = patient.UserId;
            ticket.Status = TicketStatus.Open;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            var created = await _repo.AddAsync(ticket);
            _logger.LogInformation("Support ticket {TicketId} created successfully for PatientId {PatientId}", created.TicketId, dto.PatientId);

            return _mapper.Map<SupportTicketDto>(created);
        }

        public async Task<SupportTicketDto> UpdateAsync(UpdateSupportTicketDto dto)
        {
            _logger.LogInformation("Updating support ticket {TicketId}", dto.TicketId);

            if (dto == null)
            {
                _logger.LogError("UpdateSupportTicketDto is null");
                throw new ArgumentNullException(nameof(dto));
            }

            var ticket = await _repo.GetByIdAsync(dto.TicketId);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket with ID {TicketId} not found", dto.TicketId);
                throw new KeyNotFoundException($"Ticket with ID {dto.TicketId} not found.");
            }

            if (!Enum.IsDefined(typeof(TicketStatus), dto.Status))
            {
                _logger.LogWarning("Invalid ticket status {Status} for TicketId {TicketId}", dto.Status, dto.TicketId);
                throw new ArgumentException("Invalid ticket status. Allowed values: Open, InProgress, Resolved, Closed.");
            }

            _mapper.Map(dto, ticket);
            ticket.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(ticket);
            _logger.LogInformation("Ticket {TicketId} updated successfully", dto.TicketId);

            return _mapper.Map<SupportTicketDto>(ticket);
        }

        public async Task<bool> DeleteAsync(int ticketId)
        {
            _logger.LogInformation("Deleting ticket {TicketId}", ticketId);

            var existing = await _repo.GetByIdAsync(ticketId);
            if (existing == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found", ticketId);
                return false;
            }

            await _repo.DeleteAsync(ticketId);
            _logger.LogInformation("Ticket {TicketId} deleted successfully", ticketId);
            return true;
        }

        public async Task<SupportTicketDto?> GetByIdAsync(int ticketId)
        {
            _logger.LogInformation("Fetching ticket {TicketId}", ticketId);

            var ticket = await _repo.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found", ticketId);
                return null;
            }

            return _mapper.Map<SupportTicketDto>(ticket);
        }

        public async Task<IEnumerable<SupportTicketDto>> GetAllByUserAsync(string userId)
        {
            _logger.LogInformation("Fetching all tickets for Patient UserId {UserId}", userId);
            var tickets = await _repo.GetAllByPatientIdAsync(userId);
            return _mapper.Map<IEnumerable<SupportTicketDto>>(tickets);
        }

        public async Task<IEnumerable<SupportTicketDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all support tickets in the system");
            var tickets = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<SupportTicketDto>>(tickets);
        }
    }
}
