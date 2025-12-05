using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.TestPurposeDTOs
{
    public class TestPurposeResponseDTO
    {
        public Guid TestPurposeId { get; set; }
        public string? TestPurposeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
