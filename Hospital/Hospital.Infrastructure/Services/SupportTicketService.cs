using AutoMapper;
using Hospital.Application.DTO.SupportTicket;
using Hospital.Application.Interfaces.Repos;
using Hospital.Application.Interfaces.Services;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.Services
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly ISupportTicketRepository _repo;
        private readonly IPatientRepository _PatientRepository;
        private readonly IMapper _mapper;


        public SupportTicketService(ISupportTicketRepository repo, IMapper mapper, IPatientRepository PatientRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _PatientRepository = PatientRepository;
        }

        // Patient creates a ticket
        public async Task<SupportTicketDto> CreateAsync(CreateSupportTicketDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Subject) || string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException("Subject and Message are required.");

            // 1️⃣ Get the userId from PatientId
            var patient = await _PatientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            // 2️⃣ Map DTO -> SupportTicket
            var ticket = _mapper.Map<SupportTicket>(dto);

            // 3️⃣ Set UserId, Status, timestamps manually
            ticket.UserId = patient.UserId;
            ticket.Status = Domain.Enum.TicketStatus.Open;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            // 4️⃣ Save ticket
            var created = await _repo.AddAsync(ticket);

            // 5️⃣ Return mapped DTO
            return _mapper.Map<SupportTicketDto>(created);
        }

        // Admin updates ticket
        public async Task<SupportTicketDto> UpdateAsync(UpdateSupportTicketDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var ticket = await _repo.GetByIdAsync(dto.TicketId)
                         ?? throw new KeyNotFoundException($"Ticket with ID {dto.TicketId} not found.");

            // Validate Status
            if (!Enum.IsDefined(typeof(TicketStatus), dto.Status))
                throw new ArgumentException("Invalid ticket status. Allowed values: Open, InProgress, Resolved, Closed.");

            // Only map allowed fields (Response, Status)
            _mapper.Map(dto, ticket);

            ticket.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(ticket);

            return _mapper.Map<SupportTicketDto>(ticket);
        }


        // Delete ticket
        public async Task<bool> DeleteAsync(int ticketId)
        {
            var existing = await _repo.GetByIdAsync(ticketId);
            if (existing == null) return false;

            await _repo.DeleteAsync(ticketId);
            return true;
        }

        // Get ticket by Id
        public async Task<SupportTicketDto?> GetByIdAsync(int ticketId)
        {
            var ticket = await _repo.GetByIdAsync(ticketId);
            return ticket == null ? null : _mapper.Map<SupportTicketDto>(ticket);
        }

        // Patient gets all of their tickets
        public async Task<IEnumerable<SupportTicketDto>> GetAllByUserAsync(string userId)
        {
            var tickets = await _repo.GetAllByPatientIdAsync(userId);
            return _mapper.Map<IEnumerable<SupportTicketDto>>(tickets);
        }

        // Admin gets all tickets
        public async Task<IEnumerable<SupportTicketDto>> GetAllAsync()
        {
            var tickets = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<SupportTicketDto>>(tickets);
        }
    }
}
