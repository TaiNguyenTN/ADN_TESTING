using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;

namespace Domain.Entity
{
    public class ServiceTestPurpose
    {
        public Guid Id { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Guid ServiceId { get; private set; }
        public Service Service { get; private set; }

        public Guid TestPurposeId { get; private set; }
        public TestPurpose TestPurpose { get; private set; }

        private ServiceTestPurpose() { }

        public ServiceTestPurpose(Guid serviceId, Guid testPurposeId)
        {
            Id = Guid.NewGuid();
            ServiceId = serviceId;
            TestPurposeId = testPurposeId;
        }
    }
}
