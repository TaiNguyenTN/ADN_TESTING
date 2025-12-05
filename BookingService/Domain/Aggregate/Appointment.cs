using Domain.Abstraction;
using Domain.Enum;
using Domain.ValueObject;

namespace Domain.Aggregate
{
    public class Appointment : SoftDeletedEntity
    {
        public Guid AppointmentId { get; private set; }
        public string IdentityNumber { get; private set; }
        public DateTime AppointmentDate { get; private set; }

        public string? Province { get; private set; }
        public string? District { get; private set; }
        public string? CollectionLocation { get; private set; }
        public string? FingerprintFile { get; private set; }
        public string? Note { get; private set; }

        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;
        

        // Relationship inside SAME bounded context
        public Guid ServiceId { get; private set; }
        public Service Service { get; private set; }

        public Guid TestCategoryId { get; private set; }
        public TestCategory TestCategory { get; private set; }

        public Guid TestPurposeId { get; private set; }
        public TestPurpose TestPurpose { get; private set; }

        private Appointment() { }

        public Appointment(
        string identityNumber,
        DateTime appointmentDate,
        Guid serviceId,
        Guid testCategoryId,
        Guid testPurposeId,
        string? province = null,
        string? district = null,
        string? collectionLocation = null,
        string? note = null,
        string? fingerprintFile = null)
        {
            IdentityNumber = identityNumber;
            AppointmentDate = appointmentDate;
            ServiceId = serviceId;
            TestCategoryId = testCategoryId;
            TestPurposeId = testPurposeId;

            // Optional fields
            Province = province;
            District = district;
            CollectionLocation = collectionLocation;
            Note = note;
            FingerprintFile = fingerprintFile;

            AppointmentId = Guid.NewGuid();
        }
    }
}
