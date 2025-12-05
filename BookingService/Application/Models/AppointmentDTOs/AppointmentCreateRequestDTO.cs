using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Domain.ValueObject;

namespace Application.Models.AppointmentDTOs
{
    public class AppointmentCreateRequestDTO
    {
        public string IdentityNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? CollectionLocation { get; set; }
        public string? Note { get; set; }
        public string? FingerprintFile { get; set; }
        public required string ServiceName { get; set; }
        public required string CategoryName { get; set; }
        public required string TestPurposeName { get; set; }
    }
}
