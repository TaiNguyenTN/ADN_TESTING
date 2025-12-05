using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.ValueObject;

namespace Application.Models.AppointmentDTOs
{
    public class AppointmentResponseDTO
    {
        public Guid AppointmentId { get; set; }
        public string? IdentityNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Provinces { get; set; }
        public string? District { get; set; }
        public string? CollectionLocation { get; set; }
        public string? Note { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? FingerprintFile { get; set; }

    }
}
