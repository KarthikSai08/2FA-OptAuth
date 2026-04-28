using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public record ApiResponse<T>(bool Success, string Message, T? Data = default);
}
