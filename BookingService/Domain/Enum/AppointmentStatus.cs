using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum AppointmentStatus
    {
        Pending = 1,                 // Đặt lịch nhưng chưa xác nhận
        Confirmed = 2,               // Đã xác nhận
        SampleCollectionPending = 3, // Chờ lấy mẫu
        SampleCollected = 4,         // Đã lấy mẫu
        SampleReceived = 5,          // Lab nhận mẫu
        InTesting = 6,               // Đang xét nghiệm
        Completed = 7,               // Hoàn thành xét nghiệm
        ResultReady = 8,             // Có kết quả
        ResultDelivered = 9,         // Đã trả kết quả cho khách
        Cancelled = 10,              // Hủy lịch
        SampleRejected = 11          // Mẫu bị lỗi / không hợp lệ
    }
}
