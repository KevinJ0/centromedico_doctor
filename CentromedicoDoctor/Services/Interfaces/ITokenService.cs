using Doctor.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface ITokenService
    {
        Task<IActionResult> RefreshToken(TokenRequestDTO model, bool mobile);
        Task<IActionResult> GenerateNewToken(TokenRequestDTO model, bool mobile);
    }
}
