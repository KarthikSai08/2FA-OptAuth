using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public record AuthResponse(string accessToken, string tokenType = "Bearer");
    
}
