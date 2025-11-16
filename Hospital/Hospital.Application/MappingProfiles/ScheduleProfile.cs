using AutoMapper;
using Hospital.Application.DTO.Schedule;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<CreateScheduleDto, Schedule>();
            CreateMap<UpdateScheduleDto, Schedule>();
            CreateMap<Schedule, ScheduleDto>();

        }
    }
    
}
