using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface IPacienteService
    {
        Task<bool> saveUserInfoAsync(UserInfo formuser);
        Task<bool> updateAsync(UserInfo formuser);
    }
}
