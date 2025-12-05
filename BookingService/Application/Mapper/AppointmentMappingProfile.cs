using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.AppointmentDTOs;
using AutoMapper;
using Domain.Aggregate;
using Domain.ValueObject;

namespace Application.Mapper
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<Appointment, AppointmentResponseDTO>()
                    .ForMember(dest => dest.FullName, opt => opt.Ignore())
                    .ForMember(dest => dest.Dob, opt => opt.Ignore())
                    .ForMember(dest => dest.Gender, opt => opt.Ignore())
                    .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                    .ForMember(dest => dest.Email, opt => opt.Ignore());
        }
    }
}
