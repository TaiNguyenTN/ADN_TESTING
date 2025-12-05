using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.TestCategoryDTOs
{
    public class TestCategoryResponseDTO
    {
        public Guid TestCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
    }
}
