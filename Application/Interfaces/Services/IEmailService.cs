using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOtpAsync(string email, string toName, string otpCode);
    }
}
