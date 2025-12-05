using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity;

namespace Domain.Aggregate
{
    public class TestPurpose
    {
        private readonly List<ServiceTestPurpose> _serviceTestPurposes = new();
        private readonly List<Appointment> _appointments = new();
        public Guid TestPurposeId { get; private set; }
        public string TestPurposeName { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;

        public IReadOnlyCollection<ServiceTestPurpose> ServiceTestPurposes { get; private set; }
        public IReadOnlyCollection<Appointment> Appointments { get; private set; }

        private TestPurpose() { }

        public TestPurpose(string testPurposeName, string? description)
        {
            TestPurposeId = Guid.NewGuid();
            TestPurposeName = testPurposeName;
            Description = description;
        }
    }
}
