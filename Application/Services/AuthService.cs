using Application.Dtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class AuthService(IUserRepository userRepo, IOtpRepository otpRepo,
                            IEmailService emailService, IJwtService jwtService,
                            ILogger<AuthService> logger) : IAuthService
    {
        private const int OtpExpiryMinutes = 5;
        private const int OtpLength = 6;
        public async Task<ApiResponse<string>> LoginAsync(Dtos.LoginRequestDto req)
        {
            var user = await userRepo.GetByEmailAsync(req.email);

            if(user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(req.password, user.PasswordHash))
            {
                logger.LogWarning("Failed Login attempt for {Email}", req.email);
                return new ApiResponse<string>(false, "InvalidCredentials.");
            }

            var otp = GenerateOtp();
            var expiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes);

            await otpRepo.InsertOtpAsync(user.UserId, otp, expiresAt);
            await emailService.SendOtpAsync(user.Email, user.UserName, otp);

            logger.LogInformation("Otp issued for user {UserId}", user.UserId);
            return new ApiResponse<string>(true, "Otp sent to your email. Verify within 5 minutes");
        }

        public async Task<ApiResponse<string>> ResendOtpAsync(ResendOtpRequest req)
        {
            var user = await userRepo.GetByEmailAsync(req.email);
            if (user is null || !user.IsActive)
                return new ApiResponse<string>(false, "Invalid Request.");

            var otp = GenerateOtp();
            var expiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes);

            await otpRepo.InsertOtpAsync(user.UserId, otp, expiresAt);
            await emailService.SendOtpAsync(user.Email, user.UserName, otp);

            return new ApiResponse<string>(true, "A New Otp Has been Sent to your email");

        }
        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequestDto request)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

            var newUserId = await userRepo.CreateUserAsync(request.Email, passwordHash, request.UserName);

            if (newUserId == -1)
                return new ApiResponse<string>(false, "An account with this email already exists.");

            logger.LogInformation("New user registered with Id {UserId}", newUserId);
            return new ApiResponse<string>(true, "Account created. You can now log in.");
        }
        public async Task<ApiResponse<AuthResponse>> VerifyOtpAsync(VerifyOtpRequest req)
        {
            var user = await userRepo.GetByEmailAsync(req.email);
            if (user is null || !user.IsActive)
                return new ApiResponse<AuthResponse>(false, "Invalid Request.");

            var otp = await otpRepo.GetLatestOtpAsync(user.UserId);

            if (otp is null || otp.IsUsed)
                return new ApiResponse<AuthResponse>(false, "No active Otp found. please Login Again");

            if (DateTime.UtcNow > otp.ExpiresAt)
                return new ApiResponse<AuthResponse>(false, "Otp has expired. please Login again");

            if(otp.Code != req.otp)
            {
                logger.LogWarning("Invalid Otp attempt for user {UserId}", user.UserId);
                return new ApiResponse<AuthResponse>(false, "Invalid Otp");
            }
            
            await otpRepo.MarkOtpUsedAsync(otp.OtpId);

            var token = jwtService.GenerateToken(user);
            logger.LogInformation("User {UserId} authenticated successfully", user.UserId);

            return new ApiResponse<AuthResponse>(true, "Login Successful", new AuthResponse(token));

        }


        private static string GenerateOtp()
        {
            var bytes = new byte[4];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            var number = BitConverter.ToUInt32(bytes) % 1_000_000;
            return number.ToString("D6");
        }
    }
}
