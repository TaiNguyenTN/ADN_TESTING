using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstraction;
using Domain.Entity;

namespace Domain.Aggregate
{
    public class Service : SoftDeletedEntity
    {
        private readonly List<TestCategory> categories = new();
        private readonly List<ServiceTestPurpose> serviceTestPurposes = new();
        private readonly List<Appointment> appointments = new();

        public Guid ServiceId { get; private set; }
        public string ServiceName { get; private set; }
        public string ServiceType { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; } = true;

        public IReadOnlyCollection<TestCategory> TestCategories => categories.AsReadOnly();
        public IReadOnlyCollection<ServiceTestPurpose> ServiceTestPurposes => serviceTestPurposes.AsReadOnly();
        public IReadOnlyCollection<Appointment> Appointments => appointments.AsReadOnly();

        private Service() { }

        public Service(string serviceName, string serviceType, string description, decimal price)
        {
            ServiceId = Guid.NewGuid();
            ServiceName = serviceName;
            ServiceType = serviceType;
            Description = description;
            Price = price;
        }
    }
}
