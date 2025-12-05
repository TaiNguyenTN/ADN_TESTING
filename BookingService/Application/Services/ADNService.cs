using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Interface;
using Application.Models.ServiceDTOs;
using AutoMapper;
using Domain.IRepository;

namespace Application.Services
{
    public class ADNService : IADNService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper mapper;
        public ADNService(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            this.mapper = mapper;
        }
        public async Task<GenericResult<IEnumerable<ServiceResponseDTO>>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.GetAllServicesAsync();
            var serviceDTOs = mapper.Map<IEnumerable<ServiceResponseDTO>>(services);
            return GenericResult<IEnumerable<ServiceResponseDTO>>.Success(serviceDTOs, "Services retrieved successfully.");
        }
    }
}
