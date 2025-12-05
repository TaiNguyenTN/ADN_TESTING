using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADN.IAM.Appointment.MessageBroker;
using Application.Interface;
using Domain.Aggregate;
using Domain.IRepository;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Message.IAM
{
    public class UserUpdateConsumer : IConsumer<IAMRequestUpdateDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IAMRequestUpdateDTO> _logger;

        public UserUpdateConsumer(IUnitOfWork unitOfWork, ILogger<IAMRequestUpdateDTO> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IAMRequestUpdateDTO> context)
        {
            var user = context.Message;
            _logger.LogInformation($"[MassTransit] Received IAM Update User: {user.FullName}");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var appointments = await _unitOfWork.GetRepository<IAppointmentRepository>()
                                    .GetAppointmentsByIdentityNumberAsync(user.IdentityNumber);

                if (appointments == null)
                {
                    _logger.LogWarning($"[MassTransit] No appointments found for IdentityNumber: {user.IdentityNumber}");
                    return;
                }

                // Update patient information from IAM service
                foreach (var appointment in appointments)
                {
                    _logger.LogInformation($"[MassTransit] Appointment {appointment.AppointmentId} belongs to IdentityNumber: {user.IdentityNumber}");
                }

                await _unitOfWork.CommitAsync();
                _logger.LogInformation($"[MassTransit] Successfully update user with {user.IdentityNumber}");

            }
            catch (Exception ex)
            {
                _logger.LogError($"[MassTransit] Error updating user with IdentityNumber: {user.IdentityNumber}. Exception: {ex.Message}");
                throw;
            }
        }
    }
}
