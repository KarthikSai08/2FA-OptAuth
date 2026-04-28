using Application.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<string>> LoginAsync(LoginRequestDto req);
        Task<ApiResponse<string>> ResendOtpAsync(ResendOtpRequest req);
        Task<ApiResponse<AuthResponse>> VerifyOtpAsync(VerifyOtpRequest req);
    }
}
