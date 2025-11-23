using AutoMapper;
using Hospital.Application.DTO.SupportTicket;
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class SupportTicketProfile : Profile
    {
        public SupportTicketProfile()
        {
            CreateMap<CreateSupportTicketDto, SupportTicket>()
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TicketStatus.Open))
                 .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                 .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                 .ForMember(dest => dest.UserId, opt => opt.Ignore()) 
                 .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<UpdateSupportTicketDto, SupportTicket>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<SupportTicket, SupportTicketDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(s => s.UserId))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(s => s.User.FullName));
        }
    }
}
