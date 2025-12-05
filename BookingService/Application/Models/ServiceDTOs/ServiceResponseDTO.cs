using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.TestCategoryDTOs;
using Application.Models.TestPurposeDTOs;

namespace Application.Models.ServiceDTOs
{
    public class ServiceResponseDTO
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? ServiceType { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public List<string> TestCategorieNames { get; set; } = new();
        public List<string> TestPurposeNames { get; set; } = new();
    }
}
