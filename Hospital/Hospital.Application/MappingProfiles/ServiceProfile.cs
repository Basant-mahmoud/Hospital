using AutoMapper;
using Hospital.Application.DTO.ServiceDTOS;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<CreateServiceDto, Service>();
            CreateMap<UpdateServiceDto, Service>();
            CreateMap<Service, ServiceDto>();
              
              
        }
    }
}
