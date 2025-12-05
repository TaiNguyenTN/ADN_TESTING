using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Models.ServiceDTOs;

namespace Application.Interface
{
    public interface IADNService
    {
        Task<GenericResult<IEnumerable<ServiceResponseDTO>>> GetAllServicesAsync();
    }
}
