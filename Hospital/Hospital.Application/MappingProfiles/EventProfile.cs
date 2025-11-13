using AutoMapper;
using Hospital.Application.DTO.Event;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class EventProfile: Profile
    {
        public EventProfile()
        {
            // Entity -> DTO (for reads)
            CreateMap<Event, EventDto>();

            // DTO -> Entity (for create/update)
            CreateMap<AddEventDto, Event>();

            // optionally allow two-way mapping
            CreateMap<EventDto, Event>().ReverseMap();
        }
    }
}
