using AutoMapper;
using Hospital.Application.DTO.Banner;
using Hospital.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.MappingProfiles
{
    public class BannerProfile : Profile
    {
        public BannerProfile()
        {
            CreateMap<CreateBannerDto, Banner>();
            CreateMap<UpdateBannerDto, Banner>();
            CreateMap<Banner, BannerDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));


        }

    }
}
