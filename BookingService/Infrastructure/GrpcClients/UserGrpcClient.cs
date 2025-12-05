using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Grpc.Net.Client;
using IAmService.API.gRPC;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.GrpcClients
{
    public class UserGrpcClient : IUserGrpcClient
    {
        private readonly UserGrpc.UserGrpcClient client;
        public UserGrpcClient(IConfiguration configuration)
        {
            var grpcServerUrl = Environment.GetEnvironmentVariable("GRPC_IAM_SERVICE")
                                 ?? configuration["GRPC_IAM_SERVICE"];

            if (string.IsNullOrWhiteSpace(grpcServerUrl))
                throw new Exception("GRPC_IAM_SERVICE is missing.");

            var channel = GrpcChannel.ForAddress(grpcServerUrl, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });

            client = new UserGrpc.UserGrpcClient(channel);
        }


        public async Task<bool> CheckIDExist(string identityNumber)
        {
            var response = await client.CheckUserExistsAsync(new CheckUserRequest
            {
                IdentityNumber = identityNumber
            });
            return response.Exists;
        }

        public async Task<UserDetail> GetUserDetailAsync(string identityNumber)
        {
            try
            {
                var response = await client.GetUserDetailsByIdentityNumberAsync(new GetUserDetailsRequest
                {
                    IdentityNumber = identityNumber
                });
                var userDetail = new UserDetail
                {
                    IdentityNumber = response.IdentityNumber,
                    FullName = response.FullName,
                    Dob = response.DateOfBirth?.ToDateTime(),
                    Gender = response.Gender,
                    PhoneNumber = response.PhoneNumber,
                    Email = response.Email
                };
                return userDetail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HEEEEEEEEEEEEE: {ex.Message}");
                return null;
            }
        }
    }
}
