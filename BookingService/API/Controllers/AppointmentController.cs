using Application.Helpers;
using Application.Interface;
using Application.Models.AppointmentDTOs;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserGrpcClient _userClient;
        public AppointmentController(IAppointmentService appointmentService, IUserGrpcClient userClient)
        {
            _appointmentService = appointmentService;
            _userClient = userClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDTO>>> GetAllAppointmentsAsync()
        {
            var result = await _appointmentService.GetAllAppointmentAsync();
            if (result.IsSuccess)
            {
                if (result.Data == null || !result.Data.Any())
                {
                    return Ok(GenericResult<IEnumerable<AppointmentResponseDTO>>.Success(
                        new List<AppointmentResponseDTO>(),
                        "No Data"));
                }

                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<ActionResult> AddAppointmentAsync(AppointmentCreateRequestDTO request)
        {
            var performedBy = "System";
            var result = await _appointmentService.AddAppointmentAsync(performedBy, request);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        //[HttpGet("iam/{identityNumber}")]
        //public async Task<IActionResult> TestIamGrpc(string identityNumber)
        //{
        //    try
        //    {
        //        var user = await _userClient.GetUserDetailAsync(identityNumber);
        //        return Ok(new
        //        {
        //            Success = true,
        //            IdentityNumber = user.IdentityNumber,
        //            FullName = user.FullName,
        //            Dob = user.Dob,
        //            Gender = user.Gender,
        //            PhoneNumber = user.PhoneNumber,
        //            Email = user.Email
        //        });
        //    }
        //    //catch (Grpc.Core.RpcException ex)
        //    //{
        //    //    return BadRequest(new
        //    //    {
        //    //        Success = false,
        //    //        StatusCode = ex.Status.StatusCode.ToString(), // NotFound, Internal, InvalidArgument
        //    //        Message = ex.Status.Detail
        //    //    });
        //    //}
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        });
        //    }
        //}


        [HttpGet("iam/{identityNumber}")]
        public async Task<IActionResult> TestIamGrpc(string identityNumber)
        {
            try
            {
                var user = await _userClient.GetUserDetailAsync(identityNumber);
                return Ok(new
                {
                    Success = true,
                    IdentityNumber = user.IdentityNumber,
                    FullName = user.FullName,
                    Dob = user.Dob,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email
                });
            }
            //catch (Grpc.Core.RpcException ex)
            //{
            //    return BadRequest(new
            //    {
            //        Success = false,
            //        StatusCode = ex.Status.StatusCode.ToString(), // NotFound, Internal, InvalidArgument
            //        Message = ex.Status.Detail
            //    });
            //}
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }


    }
}
