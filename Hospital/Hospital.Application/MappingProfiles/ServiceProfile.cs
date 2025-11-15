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
            CreateMap<CreateServiceDto, Service>()
                       .ForMember(dest => dest.Branches, opt => opt.Ignore());

            CreateMap<UpdateServiceDto, Service>()
                .ForMember(dest => dest.Branches, opt => opt.Ignore());

            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.BranchesID, opt =>
                    opt.MapFrom(src => src.Branches
                        .Select(b => new BranchIdDTO { BranchId = b.BranchId })
                    ));


        }
    }
}
