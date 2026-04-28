using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public record RegisterRequestDto(string Email, string Password, string UserName);
}
