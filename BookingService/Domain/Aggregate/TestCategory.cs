using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregate
{
    public class TestCategory
    {
        private readonly List<Appointment> appointments = new();
        public Guid TestCategoryId { get; private set; }
        public string CategoryName { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? Description { get; private set; }

        public Guid ServiceId { get; private set; }
        public Service Service { get; private set; }

        public IReadOnlyCollection<Appointment> Appointments => appointments.AsReadOnly();

        private TestCategory() { }

        public TestCategory(string categoryName, string? description, Guid serviceId)
        {
            TestCategoryId = Guid.NewGuid();
            CategoryName = categoryName;
            Description = description;
            ServiceId = serviceId;
        }
    }

}
