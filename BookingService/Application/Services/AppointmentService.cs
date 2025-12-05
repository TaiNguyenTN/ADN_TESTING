using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Interface;
using Application.Models.AppointmentDTOs;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;
using Domain.ValueObject;

namespace Application.Services
{
    public class AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IUserGrpcClient userClient) : IAppointmentService
    {
        public async Task<GenericResult<AppointmentResponseDTO>> AddAppointmentAsync(string performedBy, AppointmentCreateRequestDTO request)
        {
            await unitOfWork.BeginTransactionAsync();
            //Check serviceName is exist
            var existingService = await unitOfWork.GetRepository<IServiceRepository>().GetServiceByNameAsync(request.ServiceName);
            if (existingService == null)
                return GenericResult<AppointmentResponseDTO>.Failure("Service name not found.");

            var existingCatergory = await unitOfWork.GetRepository<ITestCategoryRepository>().GetTestCategoryByNameAsync(request.CategoryName);
            if (existingCatergory == null)
                return GenericResult<AppointmentResponseDTO>.Failure("Category name not found.");

            var existingTestPurpose = await unitOfWork.GetRepository<ITestPurposeRepository>().GetTestPurposeByNameAsync(request.TestPurposeName);
            if (existingTestPurpose == null)
                return GenericResult<AppointmentResponseDTO>.Failure("Test purpose name not found.");

            //Kiểm tra xem identityNumber có tồn tại bên IAmService không?
            var existed = await userClient.CheckIDExist(request.IdentityNumber);
            if(!existed)
                return GenericResult<AppointmentResponseDTO>.Failure($"User with Identity Number {request.IdentityNumber} doest not exists.");
            
            var user = await userClient.GetUserDetailAsync(request.IdentityNumber);

            var appointment = new Appointment(
            identityNumber: request.IdentityNumber,
            request.AppointmentDate,
            existingService.ServiceId,
            existingCatergory.TestCategoryId,
            existingTestPurpose.TestPurposeId,
            province: request.Province,
            district: request.District,
            collectionLocation: request.CollectionLocation,
            note: request.Note,
            fingerprintFile: request.FingerprintFile
            );

            unitOfWork.GetRepository<IAppointmentRepository>().Add(appointment);

            var result = await unitOfWork.CommitAsync();
            if(result < 0)
                return GenericResult<AppointmentResponseDTO>.Failure("Failed to create appointment.");

            var responseDTO = mapper.Map<AppointmentResponseDTO>(appointment);
            return GenericResult<AppointmentResponseDTO>.Success(responseDTO, "Appointment created successfully.");
        }

        public async Task<GenericResult<IEnumerable<AppointmentResponseDTO>>> GetAllAppointmentAsync()
        {
            var appointments = await unitOfWork.GetRepository<IAppointmentRepository>().GetAllAsync();
            var result = new List<AppointmentResponseDTO>();
            var appointmentDTOs = mapper.Map<IEnumerable<AppointmentResponseDTO>>(appointments);
            foreach (var appointment in appointments)
            {
                var appointmentDTO = mapper.Map<AppointmentResponseDTO>(appointment);
                var user = await userClient.GetUserDetailAsync(appointment.IdentityNumber);
                appointmentDTO.FullName = user.FullName;
                appointmentDTO.Dob = user.Dob;
                appointmentDTO.Gender = user.Gender;
                appointmentDTO.PhoneNumber = user.PhoneNumber;
                appointmentDTO.Email = user.Email;
                result.Add(appointmentDTO);
            }

            return GenericResult<IEnumerable<AppointmentResponseDTO>>.Success(result);
        }
    }
}
