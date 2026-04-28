using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IOtpRepository
    {
        Task<OtpRecord?> GetLatestOtpAsync(int userId);
        Task InsertOtpAsync(int userId, string code, DateTime expiresAt);
        Task MarkOtpUsedAsync(int otpId);
    }
}
