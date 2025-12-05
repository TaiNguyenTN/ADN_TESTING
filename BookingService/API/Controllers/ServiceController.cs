using Application.Helpers;
using Application.Interface;
using Application.Models.AppointmentDTOs;
using Application.Models.ServiceDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IADNService service;
        public ServiceController(IADNService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceResponseDTO>>> GetAllServicesAsync()
        {
            var result = await service.GetAllServicesAsync();
            if(result.IsSuccess)
            {
                if (result.Data == null || !result.Data.Any())
                {
                    if (result.Data == null || !result.Data.Any())
                    {
                        return Ok(GenericResult<IEnumerable<ServiceResponseDTO>>.Success(
                            new List<ServiceResponseDTO>(),
                            "No Data"));
                    }
                }
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
