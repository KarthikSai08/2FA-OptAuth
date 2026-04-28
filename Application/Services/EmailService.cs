using Application.Interfaces.Services;
using Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Application.Services
{
    public class EmailService(SmtpSettings settings, ILogger<EmailService> logger) : IEmailService
    {
        public async Task SendOtpAsync(string toEmail, string toName, string otpCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.FromName, settings.Username));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = "Your OTP";

            message.Body = new BodyBuilder
            {
                HtmlBody = BuildHtmlbody(toName, otpCode),
                TextBody = $"Your Otp is: {otpCode}. It Expires in 5 mins. Do not Share It."
            }.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                var socketOptions = settings.UseSsl
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTlsWhenAvailable;

                await client.ConnectAsync(settings.Host, settings.Port, socketOptions);
                await client.AuthenticateAsync(settings.Username, settings.Password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
            logger.LogInformation("Otp email sent to {Email}", toEmail);
        }
        private static string BuildHtmlbody(string name, string otpCode)
            => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family: Arial, sans-serif; background:#f4f4f4; padding:40px;">
                <div style="max-width:480px; margin:auto; background:#fff; border-radius:8px; padding:32px;">
                    <h2 style="color:#111;">Hello, {name}</h2>
                    <p style="color:#444;"> Use the code below to complete you login. It expires in <strong> 5 Minutes <strong>.</p>
                    <div style="text-align:center; margin:32px 0;">
                        <span style="font-size:36px; font-weight:bold; letter-spacing:12px; color:#1a1a1a;">{otpCode}</span>
                    </div>
                    <p style="color:#888; font-size:13px;">If you didn't request thism ignore this email.</p>
                </div>
            </body>
            </html>
            """;
    }
}
