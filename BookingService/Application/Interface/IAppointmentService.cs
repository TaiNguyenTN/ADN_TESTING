using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Models.AppointmentDTOs;

namespace Application.Interface
{
    public interface IAppointmentService
    {
        Task<GenericResult<IEnumerable<AppointmentResponseDTO>>> GetAllAppointmentAsync();
        Task<GenericResult<AppointmentResponseDTO>> AddAppointmentAsync(string performedBy, AppointmentCreateRequestDTO request);
    }
}
