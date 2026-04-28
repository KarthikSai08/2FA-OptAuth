using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromName { get; set; } = "Auth Service";
        public bool UseSsl { get; set; } = true;
    }
}
