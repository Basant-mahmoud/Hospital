using AutoMapper;
using Hospital.Application.DTO.Branch;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class BranchProfile : Profile
    {
        public BranchProfile()
        {
            CreateMap<Branch, BranchDto>().ReverseMap();
            CreateMap<Branch, CreateBranchDto>().ReverseMap();
            CreateMap<Branch, UpdateBranchDto>().ReverseMap();
        }
    }
}
